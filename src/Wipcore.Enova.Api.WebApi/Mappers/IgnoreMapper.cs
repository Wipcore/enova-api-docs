using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Abstractions.Interfaces;
using Wipcore.Enova.Api.Abstractions.Internal;
using Wipcore.Enova.Core;

namespace Wipcore.eNova.Api.WebApi.Mappers
{
    /// <summary>
    /// This mapper is used to ignore properties in mapping, which avoids PropertyNotFound exceptions.
    /// </summary>
    public class IgnoreMapper : IPropertyMapper
    {
        public IgnoreMapper(IConfigurationRoot configurationRoot)
        {
            var setting = configurationRoot["ApiSettings:MappingIgnoreProperties"] ?? String.Empty;
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
            return null;
        }

        public void SetEnovaProperty(BaseObject obj, string propertyName, object value, IDictionary<string, object> otherValues)
        {
        }
    }
}
