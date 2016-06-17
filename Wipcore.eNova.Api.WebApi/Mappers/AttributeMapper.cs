using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Interfaces;
using Wipcore.Enova.Core;

namespace Wipcore.eNova.Api.WebApi.Mappers
{
    /// <summary>
    /// Maps attributes belonging to any Enova object.
    /// </summary>
    public class AttributeMapper : IPropertyMapper
    {
        public List<string> Names => new List<string>() {"Attributes"};
        public Type Type => typeof (BaseObject);
        public bool InheritMapper => true;
        public int Priority => 0;
        public MapType MapType => MapType.MapFrom;

        public object MapToEnovaProperty(BaseObject obj, string propertyName, object value)
        {
            throw new NotImplementedException();
        }

        public object MapFromEnovaProperty(BaseObject obj, string propertyName)
        {
            var values = new List<object>();
            foreach (var attributeValue in obj.AttributeValues.OfType<EnovaAttributeValue>())
            {
                var value = !String.IsNullOrEmpty(attributeValue.ValueCode) ? attributeValue.ValueCode : attributeValue.Name;
                var attributeAsDictionary = new Dictionary<string, object> {{"identifier", attributeValue.Identifier}, {"Value", value},
                    { "AttributeTypeIdentifier", attributeValue.AttributeType?.Identifier }, { "AttributeTypeName", attributeValue.AttributeType?.Name } };

                values.Add(attributeAsDictionary);
            }

            return values;
        }
    }
}
