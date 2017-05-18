using System;
using System.Collections.Generic;
using System.Linq;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Abstractions.Interfaces;
using Wipcore.Enova.Core;

namespace Wipcore.eNova.Api.WebApi.Mappers.Attribute
{
    public class AttributeAsPropertyMapper : IPropertyMapper
    {
        public List<string> Names => new List<string>() {"AttributesAsProperties"};
        public Type Type => typeof(BaseObject);
        public bool InheritMapper => true;
        public int Priority => 1;
        public MapType MapType => MapType.MapFromEnovaAllowed;
        public bool PostSaveSet => false;
        public bool FlattenMapping => true;
        public object GetEnovaProperty(BaseObject obj, string propertyName, List<EnovaLanguage> mappingLanguages)
        {
            var values = new Dictionary<string, object>();
            foreach (var attributeValue in obj.AttributeValues.OfType<EnovaAttributeValue>())
            {
                var value = !String.IsNullOrEmpty(attributeValue.ValueCode) ? attributeValue.ValueCode : attributeValue.Name;
                var type = attributeValue.AttributeType?.Identifier ?? String.Empty;

                if (values.ContainsKey(type))
                    values[type] = values[type] + ", " + value;
                else
                    values.Add(type, value);
            }
            return values;
        }

        public void SetEnovaProperty(BaseObject obj, string propertyName, object value, IDictionary<string, object> otherValues)
        {
            throw new NotImplementedException();   
        }

    }
}
