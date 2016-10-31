using System;
using System.Collections.Concurrent;
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
using Wipcore.Enova.Api.WebApi.Helpers;

namespace Wipcore.Enova.Api.WebApi.Services
{
    public class MappingEnovaService : IMappingFromEnovaService, IMappingToEnovaService
    {
        private readonly IConfigurationRoot _configuration;
        private readonly IEnumerable<IPropertyMapper> _mappers;
        private readonly ConcurrentDictionary<string, IPropertyMapper> _resolvedMappers = new ConcurrentDictionary<string, IPropertyMapper>();
        private readonly ConcurrentDictionary<string, bool> _settableEnovaProperties = new ConcurrentDictionary<string, bool>();

        public MappingEnovaService(IConfigurationRoot configuration, IEnumerable<IPropertyMapper> mappers)
        {
            _configuration = configuration;
            _mappers = mappers;
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

            var dynamicObject = new Dictionary<string, object>();
            
            foreach (var property in properties.Split(','))
            {
                var mapper = GetMapper(obj.GetType(), property, MapType.MapFromEnovaAllowed);
                var value = mapper != null ? mapper.GetEnovaProperty(obj, property) : MapProperty(property, obj);
                dynamicObject.Add(property, value);
            }
            return dynamicObject;
        }

        /// <summary>
        /// Maps given properties in dictionary to the given enova object. Returns mappers that must be set after object is saved.
        /// </summary>
        public List<Action> MapToEnovaObject(BaseObject obj, IDictionary<string, object> values, List<Action> delayedMappers = null)
        {
            if (values == null)
                return delayedMappers;

            delayedMappers = delayedMappers ?? new List<Action>();

            foreach (var property in values)
            {
                var mapper = GetMapper(obj.GetType(), property.Key, MapType.MapToEnovaAllowed);
                if (mapper != null)
                {
                    if(mapper.PostSaveSet)
                        delayedMappers.Add(() => mapper.SetEnovaProperty(obj, property.Key, property.Value, values));
                    else
                        mapper.SetEnovaProperty(obj, property.Key, property.Value, values);
                }
                    //if it is a sub dictionary with additional values, from a dezerialized model for example, then map them the same way
                else if (property.IsAdditionalValuesKey())
                {
                    var subValues = ((JObject)property.Value).ToObject<Dictionary<string, object>>();
                    this.MapToEnovaObject(obj, subValues, delayedMappers);
                }
                else if (IsSettableEnovaProperty(obj.GetType(), property.Key))
                {
                    obj.SetProperty(property.Key, property.Value);
                }
            }

            return delayedMappers;
        }
        
        private bool IsSettableEnovaProperty(Type type, string propertyName)
        {
            return _settableEnovaProperties.GetOrAdd(type.FullName + propertyName, k =>
            {
                var property = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                return property != null && property.CanWrite;
            });
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
            var mapper = _resolvedMappers.GetOrAdd(type.FullName + propertyName + mapType, k =>
            {
                return _mappers.Where(x => x.MapType == MapType.MapFromAndToEnovaAllowed || x.MapType == mapType).
                Where(x => x.Names.Any(n => n.Equals(propertyName, StringComparison.CurrentCultureIgnoreCase))).
                Where(x => x.Type == type || (x.InheritMapper && x.Type.IsAssignableFrom(type))).
                OrderBy(x => x.Priority).FirstOrDefault();
            });

            return mapper;
        }
        
    }
}
