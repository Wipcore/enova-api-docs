using System;
using System.Collections.Generic;
using System.Linq;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Interfaces;
using Wipcore.Enova.Core;

namespace Wipcore.eNova.Api.WebApi.Mappers.Attribute
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
                var attributeAsDictionary = new Dictionary<string, object>
                {
                    {"Identifier", attributeValue.Identifier},
                    {"ID", attributeValue.ID},
                    {"Value", value},
                    {"ValueDescription", attributeValue.ValueDescription},
                    {
                        "AttributeType", new
                        {
                            ID = attributeValue.AttributeType?.ID,
                            Identifier = attributeValue.AttributeType?.Identifier,
                            Name = attributeValue.AttributeType?.Name,
                            TypeDescription = (attributeValue.AttributeType as EnovaAttributeType)?.TypeDescription,
                            IsContinuous = (attributeValue.AttributeType as EnovaAttributeType)?.IsContinuous,
                            Values = attributeValue.AttributeType?.Values.OfType<EnovaAttributeValue>().Select(x => new
                            {
                                Identifier = x.Identifier,
                                ID = x.ID,
                                Value = !String.IsNullOrEmpty(x.ValueCode) ? x.ValueCode : x.Name,
                                LanguageDependant = String.IsNullOrEmpty(x.ValueCode)
                            })
                        }
                    }
                }; 

                values.Add(attributeAsDictionary);
            }

            return values;
        }
    }
}
