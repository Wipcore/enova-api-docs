using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Newtonsoft.Json;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Abstractions.Interfaces;
using Wipcore.Enova.Api.Abstractions.Internal;
using Wipcore.Enova.Api.WebApi.Helpers;
using Wipcore.Enova.Core;
using Wipcore.Enova.Generics;

namespace Wipcore.eNova.Api.WebApi.Mappers.Attribute
{
    public class AttributeTypeMapper : IPropertyMapper
    {
        private readonly IContextService _context;

        public AttributeTypeMapper(IContextService context)
        {
            _context = context;
        }

        public bool PostSaveSet => true;

        public List<string> Names => new List<string>() {"Values", "LanguageDependant" };
        public Type Type => typeof (EnovaAttributeType);
        public bool InheritMapper => true;
        public bool FlattenMapping => false;
        public int Priority => 0;
        public MapType MapType => MapType.MapFromAndToEnovaAllowed;
        
        public object GetEnovaProperty(BaseObject obj, string propertyName, List<EnovaLanguage> mappingLanguages)
        {
            var type = (EnovaAttributeType)obj;
            var languageDependant = type.Values.Cast<EnovaAttributeValue>().All(x => String.IsNullOrEmpty(x.ValueCode));

            if (String.Equals(propertyName, "LanguageDependant", StringComparison.InvariantCultureIgnoreCase))
            {
                return languageDependant;
            }

            var values = new List<object>();
            foreach (var value in type.Values.Cast<EnovaAttributeValue>())
            {
                var dictionary = new Dictionary<string, object>()
                {
                    { "Identifier", value.Identifier },
                    { "ID", value.ID },
                    { "ObjectCount", value.Objects.Count }
                }.MapLanguageProperty("Value", mappingLanguages, language => languageDependant ? value.GetName(language) : value.ValueCode);

                values.Add(dictionary);
            }

            return values;
        }

        public void SetEnovaProperty(BaseObject obj, string propertyName, object value, IDictionary<string, object> otherValues)
        {
            if (String.Equals(propertyName, "LanguageDependant", StringComparison.InvariantCultureIgnoreCase))
            {
                return;//can't be set directly
            }

            var languageDependant = otherValues.GetValueInsensitive<bool>("LanguageDependant");

            var context = _context.GetContext();
            var type = (EnovaAttributeType)obj;
            foreach (var v in (IEnumerable) value)
            {
                var item = JsonConvert.DeserializeAnonymousType(v.ToString(), new { ID = 0, Identifier = "", MarkForDelete = false, Value = "", ObjectCount = 0});
                //find first by id, then identifier, then create if nothing found
                var attributeValue = (EnovaAttributeValue)(context.FindObject(item.ID, typeof(EnovaAttributeValue), false) ??
                    context.FindObject(item.Identifier ?? String.Empty, typeof(EnovaAttributeValue), false)) ??
                    EnovaObjectCreationHelper.CreateNew<EnovaAttributeValue>(context);

                if (item.MarkForDelete)
                {
                    type.DeleteValue(attributeValue);
                    continue;
                }
                attributeValue.Edit();
                attributeValue.Identifier = item.Identifier ?? String.Empty;
                if (languageDependant)
                    attributeValue.Name = item.Value;
                else
                    attributeValue.ValueCode = item.Value;

                if (attributeValue.ID > 0)
                    attributeValue.Save();

                if (type.Values.Cast<EnovaAttributeValue>().All(x => x.ID != attributeValue.ID))
                    type.AddValue(attributeValue);
            }
        }
    }
}
