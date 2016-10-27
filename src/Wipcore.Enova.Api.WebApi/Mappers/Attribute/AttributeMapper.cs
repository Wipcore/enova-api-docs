using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Interfaces;
using Wipcore.Enova.Api.WebApi.Helpers;
using Wipcore.Enova.Core;

namespace Wipcore.eNova.Api.WebApi.Mappers.Attribute
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
        public MapType MapType => MapType.MapFromAndToEnovaAllowed;

        public void SetEnovaProperty(BaseObject obj, string propertyName, object value, IDictionary<string, object> otherValues)
        {
            if (value == null)
                return;

            var context = obj.GetContext();
            var product = (EnovaBaseProduct)obj;
            dynamic attributes = value;
            foreach (var a in attributes)
            {
                var attribute = JsonConvert.DeserializeAnonymousType(a.ToString(), new {
                    Identifier = "",
                    ID = 0,
                    Value = "",
                    ValueDescription = "",
                    MarkForDelete = false,
                    LanguageDependant = false,
                    AttributeType = new
                    {
                        ID = 0,
                        Identifier = "",
                        Name = "",
                        TypeDescription = "",
                        IsContinuous = false,
                        Values = new []{ new {
                            Identifier = "",
                            ID = 0,
                            Value = "",
                            LanguageDependant = false
                        }}
                    }
                });

                var enovaAttribute = context.FindObject(Convert.ToInt32(attribute.ID), typeof(EnovaAttributeValue), false) as EnovaAttributeValue;
                var isNew = enovaAttribute == null;

                if (attribute.MarkForDelete)
                {
                    if (isNew)
                        continue;

                    product.RemoveAttributeValue(enovaAttribute);
                    continue;
                }

                if (attribute.AttributeType?.IsContinuous == true) //if contineous then just add whatever value it is
                {
                    if (isNew)
                        enovaAttribute = EnovaObjectCreationHelper.CreateNew<EnovaAttributeValue>(context);
                    else
                        enovaAttribute.Edit();

                    if (attribute.Value != null)
                    {
                        if (attribute.LanguageDependant)
                            enovaAttribute.Name = attribute.Value;
                        else
                            enovaAttribute.ValueCode = attribute.Value;
                    }

                    if (!isNew) //if not new, then save it
                    {
                        enovaAttribute.Save();
                    }
                    else //otherwise set up the attribute type
                    {
                        var attributeType = EnovaAttributeType.Find(context, Convert.ToInt32(attribute.AttributeType.ID));
                        attributeType.AddValue(enovaAttribute);
                        product.AddAttributeValue(enovaAttribute);
                    }
                }
                else //if not contineous then add the new attribute and remove the old
                {
                    var currentValue = enovaAttribute == null ? null : !String.IsNullOrEmpty(enovaAttribute.ValueCode) ? enovaAttribute.ValueCode : enovaAttribute.Name;
                    var requestedNewValue = attribute.Value;

                    if (!requestedNewValue.Equals(currentValue)) //if the value is changed
                    {
                        //then find the correct attribute
                        var attributeType = (EnovaAttributeType)EnovaAttributeType.Find(context, Convert.ToInt32(attribute.AttributeType.ID));

                        var newAttributeValue = attributeType.Values.OfType<EnovaAttributeValue>().
                            First(x => x.ValueCode == requestedNewValue || x.Name == requestedNewValue);

                        if (enovaAttribute != null)
                            product.RemoveAttributeValue(enovaAttribute);

                        product.AddAttributeValue(newAttributeValue);
                    }
                }
            }
        }
    

        public object GetEnovaProperty(BaseObject obj, string propertyName)
        {
            var values = new List<object>();
            foreach (var attributeValue in obj.AttributeValues.OfType<EnovaAttributeValue>())
            {
                var value = !String.IsNullOrEmpty(attributeValue.ValueCode) ? attributeValue.ValueCode : attributeValue.Name;
                var attribute = new 
                {
                    Identifier = attributeValue.Identifier,
                    ID = attributeValue.ID,
                    Value = value,
                    ValueDescription = attributeValue.ValueDescription,
                    AttributeType = new
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
                    };

                values.Add(attribute);
            }

            return values;
        }
    }
}
