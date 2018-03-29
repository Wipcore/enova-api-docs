using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Abstractions.Interfaces;
using Wipcore.Enova.Api.Abstractions.Internal;
using Wipcore.Enova.Api.WebApi.Helpers;
using Wipcore.Enova.Core;

namespace Wipcore.eNova.Api.WebApi.Mappers.Manufacturer
{
    public class ManufacturerProductsMapper : IPropertyMapper
    {
        public List<string> Names => new List<string>() { "Products" };
        public Type Type => typeof(EnovaManufacturer);
        public bool InheritMapper => true;
        public int Priority => 0;
        public MapType MapType => MapType.MapFromAndToEnovaAllowed;
        public bool PostSaveSet => false;
        public bool FlattenMapping => false;
        public object GetEnovaProperty(BaseObject obj, string propertyName, List<EnovaLanguage> mappingLanguages)
        {
            var manufacturer = (EnovaManufacturer)obj;
            return manufacturer.GetItems(typeof (EnovaBaseProduct)).Cast<EnovaBaseProduct>().Select(x => new Dictionary<string, object>()
            {
                {"ID", x.ID }, {"Identifier", x.Identifier}
                
            }.MapLanguageProperty("Name", mappingLanguages, x.GetName));
        }

        public void SetEnovaProperty(BaseObject obj, string propertyName, object value, IDictionary<string, object> otherValues)
        {
            if(value == null)
                return;

            var context = obj.GetContext();
            var manufacturer = (EnovaManufacturer)obj;
            var productModels = JsonConvert.DeserializeAnonymousType(value.ToString(), new[] { new { ID = 0, Identifier = "", MarkForDelete = false } });

            foreach (var productModel in productModels)
            {
                var product = context.Find<EnovaBaseProduct>(productModel.ID, productModel.Identifier, true);
                if (productModel.MarkForDelete)
                {
                    if (manufacturer.HasItem(product))
                        manufacturer.RemoveItem(product);
                }
                else
                {
                    if (!manufacturer.HasItem(product))
                        manufacturer.AddItem(product);
                }
            }

        }

    }
}
