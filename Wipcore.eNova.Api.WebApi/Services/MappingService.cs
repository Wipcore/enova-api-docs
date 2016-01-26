using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Generics;
using Wipcore.Enova.Api.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Runtime.Caching;

namespace Wipcore.eNova.Api.WebApi.Services
{
    public class MappingService : IMappingService
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

        public IEnumerable<IDictionary<string, object>> GetProperties(BaseObjectList objects, string properties)
        {
            foreach (BaseObject obj in objects)//TODO parallel
            {
                var dynamicObject = GetProperties(obj, properties);
                yield return dynamicObject;
            }
        }

        public IDictionary<string, object> GetProperties(BaseObject obj, string properties)
        {
            var dynamicObject = new Dictionary<string, Object>();

            if (String.IsNullOrEmpty(properties))
            {
                // Get properties from configuration
                properties = _configuration["properties:product"] ?? "Identifier"; //TODO not just products
            }

            foreach (var property in properties.Split(','))
            {
                var mapper = GetMapper(obj.GetType(), property);
                var value = mapper != null ? mapper.Map(obj) : obj.GetProperty(property);
                dynamicObject.Add(property, value);
            }
            return dynamicObject;
        }

        private IPropertyMapper GetMapper(Type type, string propertyName)
        {
            var key = type.FullName + propertyName;

            var lazyMapper = new Lazy<IPropertyMapper>(() => {
                return  _mappers.
                Where(x => x.Name == propertyName.ToLower()).
                Where(x => x.Type == type || (x.InheritMapper && x.Type.IsAssignableFrom(type))).
                OrderBy(x => x.Priority).FirstOrDefault();                
            });

            var cachedMapper = (Lazy<IPropertyMapper>)_cache.AddOrGetExisting(key, lazyMapper, DateTime.Now.AddMinutes(15));

            return (cachedMapper ?? lazyMapper).Value;            
        }
    }
}
