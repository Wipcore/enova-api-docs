using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Interfaces;
using Wipcore.Enova.Core;

namespace Wipcore.eNova.Api.WebApi.Mappers.Attribute
{
    public class AttributeTypeMapper : IPropertyMapper
    {
        public List<string> Names => new List<string>() { "Values" };
        public Type Type => typeof(EnovaAttributeType);
        public bool InheritMapper => true;
        public int Priority => 0;
        public MapType MapType => MapType.MapFrom;

        public void MapToEnovaProperty(BaseObject obj, string propertyName, object value, IDictionary<string, object> otherValues)
        {
            throw new NotImplementedException();
        }

        public object MapFromEnovaProperty(BaseObject obj, string propertyName)
        {
            var type = (EnovaAttributeType) obj;
            var values = type.Values.Cast<EnovaAttributeValue>().Select(x => new
            {
                Identifier = x.Identifier,
                ID = x.ID,
                Name = x.Name,
                Value = !String.IsNullOrEmpty(x.ValueCode) ? x.ValueCode : x.Name,
                LanguageDependant = String.IsNullOrEmpty(x.ValueCode)
            });

            return values;
        }
    }
}
