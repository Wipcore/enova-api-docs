using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Newtonsoft.Json;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Abstractions.Interfaces;
using Wipcore.Enova.Core;

namespace Wipcore.eNova.Api.WebApi.Mappers.Section
{
    public class SectionProdctsMapper : IPropertyMapper
    {
        public bool PostSaveSet => false;
        public List<string> Names => new List<string>() { "Products" };
        public Type Type => typeof(EnovaBaseProductSection);
        public bool InheritMapper => true;
        public int Priority => 0;
        public bool FlattenMapping => false;
        public MapType MapType => MapType.MapFromAndToEnovaAllowed;

        public object GetEnovaProperty(BaseObject obj, string propertyName, List<EnovaLanguage> mappingLanguages)
        {
            var section = (EnovaBaseProductSection)obj;
            
            return section?.Items.OfType<EnovaBaseProduct>().Select(x => new Dictionary<string, object>() { {"ID", x.ID}, {"Identifier", x.Identifier}, { "MarkForDelete", false } }.MapLanguageProperty("Name", mappingLanguages, x.GetName));
        }
      
        public void SetEnovaProperty(BaseObject obj, string propertyName, object value, IDictionary<string, object> otherValues)
        {
            if (value == null)
                return;

            var section = (EnovaBaseProductSection)obj;

            var productModels = JsonConvert.DeserializeAnonymousType(value.ToString(), new[] { new { Identifier = "", MarkForDelete = false } });

            foreach (var productModel in productModels)
            {
                var product = EnovaBaseProduct.Find(section.GetContext(), productModel.Identifier);
                if (productModel.MarkForDelete)
                {

                    if (section.HasItem(product))
                        section.RemoveItem(product);
                }
                else
                {
                    if (!section.HasItem(product))
                        section.AddItem(product);
                }
            }
        }
    }

}
