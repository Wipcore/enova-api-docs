﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Generics;
using Wipcore.Enova.Api.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Runtime.Caching;
using Fasterflect;

namespace Wipcore.Enova.Api.WebApi.Services
{
    public class MappingService : IMappingFromService, IMappingToService
    {
        private readonly IConfigurationRoot _configuration;
        private readonly IEnumerable<IPropertyMapper> _mappers;
        private readonly ObjectCache _cache;

        public MappingService(IConfigurationRoot configuration, ObjectCache cache, IEnumerable<IPropertyMapper> mappers)
        {
            _configuration = configuration;
            _mappers = mappers;
            _cache = cache;
        }

        public IEnumerable<IDictionary<string, object>> MapFrom(BaseObjectList objects, string properties)
        {
            foreach (BaseObject obj in objects)//TODO parallel
            {
                var dynamicObject = MapFrom(obj, properties);
                yield return dynamicObject;
            }
        }

        public IDictionary<string, object> MapFrom(BaseObject obj, string properties)
        {
            if (properties == null)
                properties = "identifier";

            var dynamicObject = new Dictionary<string, Object>(); //TODO map nested objects somehow
            
            foreach (var property in properties.Split(','))
            {
                var mapper = GetMapper(obj.GetType(), property, MapType.MapFrom);
                var value = mapper != null ? mapper.MapFrom(obj) : MapProperty(property.Split('.'), obj);
                dynamicObject.Add(property, value);
            }
            return dynamicObject;
        }

        public IDictionary<string, object> MapTo(BaseObject obj, IDictionary<string, object> values)
        {
            foreach (var property in values)
            {
                var mapper = GetMapper(obj.GetType(), property.Key, MapType.MapTo);
                if (mapper != null)
                {
                    var mappedValue = mapper.MapTo(obj);
                    values[property.Key] = mappedValue;
                }
                else
                    obj.SetProperty(property.Key, property.Value);
            }

            return values;
        }

        private object MapProperty(string[] propertyNames, BaseObject obj)
        {
            for (var i = 0; i < propertyNames.Length; i++)
            {
                if (i == propertyNames.Length - 1)
                    return obj.GetProperty(propertyNames[i]);
                obj = obj.GetPropertyValue(propertyNames[i], BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance) as BaseObject;

                if (obj == null)
                    break;
            }
            return null;
        }

        private IPropertyMapper GetMapper(Type type, string propertyName, MapType mapType)
        {
            var key = type.FullName + propertyName;

            var lazyMapper = new Lazy<IPropertyMapper>(() => {
                return  _mappers.
                Where(x => x.MapType == MapType.MapAll || x.MapType == mapType).
                Where(x => x.Name == propertyName.ToLower()).
                Where(x => x.Type == type || (x.InheritMapper && x.Type.IsAssignableFrom(type))).
                OrderBy(x => x.Priority).FirstOrDefault();                
            });

            var cachedMapper = (Lazy<IPropertyMapper>)_cache.AddOrGetExisting(key, lazyMapper, DateTime.Now.AddMinutes(15));

            return (cachedMapper ?? lazyMapper).Value;            
        }

        
    }
}
