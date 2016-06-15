using System;
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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Wipcore.eNova.Api.WebApi.Helpers;

namespace Wipcore.Enova.Api.WebApi.Services
{
    public class MappingEnovaService : IMappingFromEnovaService, IMappingToEnovaService
    {
        private readonly IConfigurationRoot _configuration;
        private readonly IEnumerable<IPropertyMapper> _mappers;
        private readonly ObjectCache _cache;

        public MappingEnovaService(IConfigurationRoot configuration, ObjectCache cache, IEnumerable<IPropertyMapper> mappers)
        {
            _configuration = configuration;
            _mappers = mappers;
            _cache = cache;
        }

        /// <summary>
        /// Maps given enova objects with the comma-seperated properties into dictionaries.
        /// </summary>
        public IEnumerable<IDictionary<string, object>> MapFromEnovaObject(BaseObjectList objects, string properties)
        {
            return from BaseObject obj in objects select MapFromEnovaObject(obj, properties);
        }

        /// <summary>
        /// Maps given enova object with the comma-seperated properties into a dictionary of property-value.
        /// </summary>
        public IDictionary<string, object> MapFromEnovaObject(BaseObject obj, string properties)
        {
            if (properties == null)
                properties = "identifier";

            var dynamicObject = new Dictionary<string, Object>();
            
            foreach (var property in properties.Split(','))
            {
                var mapper = GetMapper(obj.GetType(), property, MapType.MapFrom);
                var value = mapper != null ? mapper.MapFromEnovaProperty(obj, property) : MapProperty(property, obj);
                dynamicObject.Add(property, value);
            }
            return dynamicObject;
        }

        /// <summary>
        /// Maps given properties in dictionary to the given enova object.
        /// </summary>
        public IDictionary<string, object> MapToEnovaObject(BaseObject obj, IDictionary<string, object> values)
        {
            if (values == null)
                return values;

            foreach (var property in values)
            {
                var mapper = GetMapper(obj.GetType(), property.Key, MapType.MapTo);
                if (mapper != null)
                {
                    var mappedValue = mapper.MapToEnovaProperty(obj, property.Key);
                    values[property.Key] = mappedValue;
                }
                    //if it is a sub dictionary with additional values, from a dezerialized model for example, then map them the same way
                else if (property.IsAdditionalValuesKey())
                {
                    var subValues = ((JObject)property.Value).ToObject<Dictionary<string, object>>();
                    this.MapToEnovaObject(obj, subValues);
                }
                else
                    obj.SetProperty(property.Key, property.Value);
            }

            return values;
        }

        private object MapProperty(string property, BaseObject obj)
        {
            var properties = property.Split('.'); //splitting ex. Manufacturer.Identifier into its parts
            for (var i = 0; i < properties.Length; i++)
            {
                if (i == properties.Length - 1) //the last name (Identifier in example above) is returned directly
                    return obj.GetProperty(properties[i]);

                //nested properties are retrieved from the object. In example obj is set to Manufacturer
                obj = obj.GetPropertyValue(properties[i], BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance) as BaseObject;

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
                Where(x => x.Names.Any(n => n.Equals(propertyName, StringComparison.CurrentCultureIgnoreCase))).
                Where(x => x.Type == type || (x.InheritMapper && x.Type.IsAssignableFrom(type))).
                OrderBy(x => x.Priority).FirstOrDefault();                
            });

            var cachedMapper = (Lazy<IPropertyMapper>)_cache.AddOrGetExisting(key, lazyMapper, DateTime.Now.AddMinutes(15));

            return (cachedMapper ?? lazyMapper).Value;            
        }
        
    }
}
