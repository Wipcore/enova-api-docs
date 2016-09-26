using System;
using System.Collections.Generic;
using System.Linq;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Interfaces;
using Wipcore.Enova.Api.WebApi.Helpers;
using Wipcore.Enova.Core;

namespace Wipcore.eNova.Api.WebApi.Mappers
{
    /// <summary>
    /// Maps attributes belonging to any Enova object.
    /// </summary>
    public class AttributeMapper : IPropertyMapper
    {
        public List<string> Names => new List<string>() { "Attributes" };
        public Type Type => typeof(BaseObject);
        public bool InheritMapper => true;
        public int Priority => 0;
        public MapType MapType => MapType.MapAll;

        public void MapToEnovaProperty(BaseObject obj, string propertyName, object value, IDictionary<string, object> otherValues)
        {
            if (value == null)
                return;

            var context = obj.GetContext();
            var product = (EnovaBaseProduct)obj;
            dynamic attributes = value;
            foreach (var attributeJson in attributes)
            {
                var enovaAttribute = context.FindObject(Convert.ToInt32(attributeJson.ID.Value), typeof(EnovaAttributeValue), false) as EnovaAttributeValue;
                var isNew = enovaAttribute == null;

                if (attributeJson.MarkForDelete.Value)
                {
                    if (isNew)
                        continue;

                    product.RemoveAttributeValue(enovaAttribute);
                    continue;
                }

                if (attributeJson.AttributeType?.IsContinuous?.Value) //if contineous then just add whatever value it is
                {
                    if (isNew)
                        enovaAttribute = EnovaObjectCreationHelper.CreateNew<EnovaAttributeValue>(context);
                    else
                        enovaAttribute.Edit();
                    enovaAttribute.Identifier = attributeJson.Identifier.Value;

                    if (attributeJson.Value != null)
                    {
                        if (attributeJson.LanguageDependant.Value)
                            enovaAttribute.Name = attributeJson.Value.Value;
                        else
                            enovaAttribute.ValueCode = attributeJson.Value.Value;
                    }

                    if (!isNew) //if not new, then save it
                    {
                        enovaAttribute.Save();
                    }
                    else //otherwise set up the attribute type
                    {
                        var attributeType = EnovaAttributeType.Find(context, Convert.ToInt32(attributeJson.AttributeType.ID));
                        attributeType.AddValue(enovaAttribute);
                        product.AddAttributeValue(enovaAttribute);
                    }
                }
                else //if not contineous then add the new attribute and remove the old
                {
                    var currentValue = enovaAttribute == null ? null : !String.IsNullOrEmpty(enovaAttribute.ValueCode) ? enovaAttribute.ValueCode : enovaAttribute.Name;
                    var requestedNewValue = attributeJson.Value.Value;

                    if (!requestedNewValue.Equals(currentValue)) //if the value is changed
                    {
                        //then find the correct attribute
                        var attributeType = (EnovaAttributeType)EnovaAttributeType.Find(context, Convert.ToInt32(attributeJson.AttributeType.ID));

                        var newAttributeValue = attributeType.Values.OfType<EnovaAttributeValue>().
                            First(x => x.ValueCode == requestedNewValue || x.Name == requestedNewValue);

                        if (enovaAttribute != null)
                            product.RemoveAttributeValue(enovaAttribute);

                        product.AddAttributeValue(newAttributeValue);
                    }
                }
            }
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
