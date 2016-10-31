using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Interfaces;
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

        public object GetEnovaProperty(BaseObject obj, string propertyName)
        {
            var manufacturer = (EnovaManufacturer)obj;
            return manufacturer.GetItems(typeof (EnovaBaseProduct)).Cast<EnovaBaseProduct>().Select(x => new {x.ID, x.Identifier, x.Name});
        }

        public void SetEnovaProperty(BaseObject obj, string propertyName, object value, IDictionary<string, object> otherValues)
        {
            var context = obj.GetContext();
            var manufacturer = (EnovaManufacturer)obj;
            var productModels = JsonConvert.DeserializeAnonymousType(value.ToString(), new[] { new { ID = 0, MarkForDelete = false } });

            foreach (var productModel in productModels)
            {
                var product = EnovaBaseProduct.Find(context, productModel.ID);
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
