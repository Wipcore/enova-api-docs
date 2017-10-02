using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Abstractions.Interfaces;
using Wipcore.Enova.Api.Abstractions.Internal;
using Wipcore.Enova.Core;

namespace Wipcore.eNova.Api.WebApi.Mappers
{
    /// <summary>
    /// This mapper is used to avoid saving a property directly on the Enova object (if the property actually dosen't exist for example) and instead saves it in the cache to be returned at next get request. WIP.
    /// </summary>
    public class RepeaterMapper : IPropertyMapper
    {
        private readonly ObjectCache _cache;

        public RepeaterMapper(IConfigurationRoot configurationRoot, ObjectCache cache)
        {
            _cache = cache;
            var setting = configurationRoot["ApiSettings:MappingRepeaterProperties"] ?? "";
            Names = setting.Split(',').Select(x => x.Trim()).ToList();
        }

        public bool PostSaveSet => false;
        public List<string> Names { get; }
        public Type Type => typeof(BaseObject);
        public bool InheritMapper => true;
        public int Priority => 0;
        public bool FlattenMapping => false;
        public MapType MapType => MapType.MapFromAndToEnovaAllowed;

        public object GetEnovaProperty(BaseObject obj, string propertyName, List<EnovaLanguage> mappingLanguages)
        {
            var idKey = $"{obj.ID}{propertyName}";
            var identifierKey = $"{obj.Identifier}{propertyName}";

            return _cache.Contains(idKey)
                ? _cache[idKey]
                : _cache.Contains(identifierKey) ? _cache[identifierKey] : null;
        }

        public void SetEnovaProperty(BaseObject obj, string propertyName, object value, IDictionary<string, object> otherValues)
        {
            if(obj.ID == 0 && String.IsNullOrEmpty(obj.Identifier))
                return;

            var key = obj.ID > 0 ? $"{obj.ID}{propertyName}" : $"{obj.Identifier}{propertyName}";
            _cache.Set(key, value, DateTime.Now.AddMinutes(1));//just basically the same request
        }
    }
}
