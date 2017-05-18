using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Abstractions.Interfaces;
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
        public bool FlattenMapping => false;
        public int Priority => 0;
        public MapType MapType => MapType.MapFromAndToEnovaAllowed;

        public bool PostSaveSet => false;
        
        public object GetEnovaProperty(BaseObject obj, string propertyName, List<EnovaLanguage> mappingLanguages)
        {
            var values = new List<object>();

            foreach (var attributeValue in obj.AttributeValues.OfType<EnovaAttributeValue>())
            {
                var attributeType = attributeValue.AttributeType as EnovaAttributeType;
                var languageDependant = attributeType?.Values.Cast<EnovaAttributeValue>().All(x => String.IsNullOrEmpty(x.ValueCode)) == true;

                var attribute = new Dictionary<string, object>()
                    {
                        {"Identifier", attributeValue.Identifier},
                        { "ID", attributeValue.ID }
                    }.
                    MapLanguageProperty("Value", mappingLanguages, language => languageDependant ? attributeValue.GetName(language) : attributeValue.ValueCode);
                
                if (attributeType != null)
                {
                    attribute.Add("AttributeType", new Dictionary<string, object>()
                        {
                            { "ID", attributeType.ID},
                            { "Identifier", attributeType.Identifier},
                            { "IsContinuous", attributeType.IsContinuous},
                            { "LanguageDependant", languageDependant},
                            { "Values", attributeType.Values.OfType<EnovaAttributeValue>().Select(x => new Dictionary<string, object>()
                                {
                                    { "Identifier", x.Identifier},
                                    { "ID", x.ID },
                                }.
                                MapLanguageProperty("Value", mappingLanguages, language => languageDependant ? x.GetName(language) : x.ValueCode))
                            }
                        }
                        .MapLanguageProperty("Name", mappingLanguages,language => attributeType.GetName(language))
                        .MapLanguageProperty("TypeDescription", mappingLanguages,language => attributeType.GetTypeDescription(language)));
                }

                values.Add(attribute);
            }
            
            return values;
        }

        public void SetEnovaProperty(BaseObject obj, string propertyName, object value, IDictionary<string, object> otherValues)
        {
            if (value == null)
                return;

            var context = obj.GetContext();
            var product = (EnovaBaseProduct)obj;
            dynamic attributes = value;
            foreach (var a in attributes)
            {
                var attribute = JsonConvert.DeserializeAnonymousType(a.ToString(), new
                {
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
                        Values = new[]{ new {
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
                        var attributeType = (EnovaAttributeType)(context.FindObject(Convert.ToInt32(attribute.AttributeType.ID), typeof(EnovaAttributeType), false) ??
                            context.FindObject(attribute.AttributeType.Identifier, typeof(EnovaAttributeType), true));

                        var newAttributeValue = attributeType.Values.OfType<EnovaAttributeValue>().
                            First(x => x.ValueCode == requestedNewValue || x.Name == requestedNewValue);

                        if (enovaAttribute != null)
                            product.RemoveAttributeValue(enovaAttribute);

                        product.AddAttributeValue(newAttributeValue);
                    }
                }
            }
        }
    }
}
