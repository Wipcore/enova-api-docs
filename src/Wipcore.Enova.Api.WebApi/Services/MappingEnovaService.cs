using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using Fasterflect;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Abstractions.Interfaces;
using Wipcore.Enova.Api.Abstractions.Internal;
using Wipcore.Enova.Api.WebApi.Helpers;
using Wipcore.Enova.Core;
using Wipcore.Enova.Generics;
using Wipcore.Library;

namespace Wipcore.Enova.Api.WebApi.Services
{
    public class MappingEnovaService : IMappingFromEnovaService, IMappingToEnovaService
    {
        private readonly IConfigurationRoot _configuration;
        private readonly IEnumerable<IPropertyMapper> _mappers;
        private readonly IContextService _contextService;
        private readonly ConcurrentDictionary<string, IPropertyMapper> _resolvedMappers = new ConcurrentDictionary<string, IPropertyMapper>();
        private readonly ConcurrentDictionary<string, bool> _settableEnovaProperties = new ConcurrentDictionary<string, bool>();

        public MappingEnovaService(IConfigurationRoot configuration, IEnumerable<IPropertyMapper> mappers, IContextService contextService)
        {
            _configuration = configuration;
            _mappers = mappers;
            _contextService = contextService;
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
            var objType = obj.GetType();
            var context = _contextService.GetContext();
            
            foreach (var property in properties.Split(',').Select(x => x.Trim()).Distinct())
            {
                var mapper = GetMapper(objType, property, MapType.MapFromEnovaAllowed);
                if (mapper == null)
                {
                    var value = MapPropertyDirect(property, obj);
                    dynamicObject.Add(property, value);
                }
                else
                {
                    MapPropertyFromMapper(property, obj, mapper, context, dynamicObject);
                }
            }
            
            return dynamicObject;
        }

        private void MapPropertyFromMapper(string property, BaseObject obj, IPropertyMapper mapper, Context context, IDictionary<string, object> dynamicObject)
        {
            var propertyAndLanguage = property.Split('-');//if language specified, for example name-en for english name
            var languages = propertyAndLanguage.Length == 1 ? null : context.FindObjects<EnovaLanguage>(propertyAndLanguage[1].Split(';')).ToList();

            var value = mapper.GetEnovaProperty(obj, propertyAndLanguage[0], languages);

            if (mapper.FlattenMapping)//add values directly to object instead of as a subvalue. AttributesAsProperties for example.
            {
                var subValues = (IDictionary<string, object>)value;
                subValues.ForEach(x => dynamicObject.Add(x.Key, x.Value));
            }
            else
            {
                dynamicObject.Add(propertyAndLanguage[0], value);
            }
        }

        private object MapPropertyDirect(string property, BaseObject obj)
        {
            var properties = property.Split('.'); //splitting ex. Manufacturer.Identifier into its parts
            for (var i = 0; i < properties.Length; i++)
            {
                if (i == properties.Length - 1) //the last name (Identifier in example above) is returned directly
                {
                    var propertyAndLanguage = properties[i].Split('-');
                    return propertyAndLanguage.Length == 1 ?
                        obj.GetProperty(properties[i]) :
                        obj.GetProperty(propertyAndLanguage[0], EnovaLanguage.Find(obj.GetContext(), propertyAndLanguage[1]));
                }

                //nested properties are retrieved from the object. In example obj is set to Manufacturer
                obj = obj.GetPropertyValue(properties[i], BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance) as BaseObject;

                if (obj == null)
                    break;
            }
            return null;
        }


        private IPropertyMapper GetMapper(Type type, string propertyName, MapType mapType)
        {
            var propertyNameSansLanguage = propertyName.Split('-').First();
            var mapper = _resolvedMappers.GetOrAdd(type.FullName + propertyNameSansLanguage + mapType, k =>
            {
                return _mappers.Where(x => x.MapType == MapType.MapFromAndToEnovaAllowed || x.MapType == mapType).
                Where(x => x.Names.Any(n => n.Equals(propertyNameSansLanguage, StringComparison.CurrentCultureIgnoreCase))).
                Where(x => x.Type == type || (x.InheritMapper && x.Type.IsAssignableFrom(type))).
                OrderBy(x => x.Priority).FirstOrDefault();
            });

            return mapper;
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

            //orders and carts might need to be recalculated if their rows have changed
            if(obj is EnovaCart cart)
                Recalculate(cart);
            (obj as EnovaOrder)?.Recalculate();

            return delayedMappers;
        }

        private void Recalculate(EnovaCart cart)
        {
            try
            {
                cart.Recalculate();
            }
            catch (NullReferenceException)
            {
                //if null reference error, it might be a deleted product
                var deletedProductRow = cart.GetCartItems<EnovaProductCartItem>().FirstOrDefault(x => x.Product == null);

                if (deletedProductRow == null)
                    throw;

                throw new HttpException(HttpStatusCode.BadRequest, $"Cannot recalculate cart {cart.ID}. It has a product with identifier {deletedProductRow.ProductIdentifier} that has been deleted.");
            }
        }

        /// <summary>
        /// Clear the cache of a property that has been removed.
        /// </summary>
        public void ClearSettablePropertyCache(Type type, string propertyName)
        {
            _settableEnovaProperties.TryRemove(type.FullName + propertyName, out _);
        }
        
        private bool IsSettableEnovaProperty(Type type, string propertyName)
        {
            return _settableEnovaProperties.GetOrAdd(type.FullName + propertyName, k =>
            {
                var property = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                return property != null ? property.CanWrite : 
                    _contextService.GetContext().GetAllPropertyNames(type, out _).Contains(propertyName, StringComparer.CurrentCultureIgnoreCase);//possible dynamic property
            });
        }

        
        
    }
}
