using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Newtonsoft.Json;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Abstractions.Interfaces;
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

        public List<string> Names => new List<string>() {"Values"};
        public Type Type => typeof (EnovaAttributeType);
        public bool InheritMapper => true;
        public bool FlattenMapping => false;
        public int Priority => 0;
        public MapType MapType => MapType.MapFromAndToEnovaAllowed;
        
        public object GetEnovaProperty(BaseObject obj, string propertyName, List<EnovaLanguage> mappingLanguages)
        {
            var type = (EnovaAttributeType)obj;
            var values = new List<object>();
            foreach (var value in type.Values.Cast<EnovaAttributeValue>())
            {
                var languageDependant = String.IsNullOrEmpty(value.ValueCode);

                var expando = new Dictionary<string, object>()
                {
                    { "Identifier", value.Identifier },
                    { "ID", value.ID },
                    { "LanguageDependant", languageDependant },
                    { "ObjectCount", value.Objects.Count }
                }.MapLanguageProperty("Value", mappingLanguages, language => languageDependant ? value.GetName(language) : value.ValueCode);

                values.Add(expando);
            }

            return values;
        }

        public void SetEnovaProperty(BaseObject obj, string propertyName, object value, IDictionary<string, object> otherValues)
        {
            var context = _context.GetContext();
            var type = (EnovaAttributeType)obj;
            foreach (var v in value as dynamic)
            {
                var item = JsonConvert.DeserializeAnonymousType(v.ToString(), new { ID = 0, Identifier = "", MarkForDelete = false, Value = "", ObjectCount = 0, LanguageDependant = false });
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
                if (item.LanguageDependant)
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
