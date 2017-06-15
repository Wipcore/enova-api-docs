using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Abstractions.Interfaces;
using Wipcore.Enova.Api.WebApi.Helpers;
using Wipcore.Enova.Core;
using Wipcore.Enova.Generics;

namespace Wipcore.eNova.Api.WebApi.Mappers.Cart
{
    public class ProductCartItemMapper : IPropertyMapper
    {
        public bool PostSaveSet => false;
        public bool InheritMapper => true;

        public int Priority => 0;
        public MapType MapType => MapType.MapFromAndToEnovaAllowed;

        public List<string> Names => new List<string>() { "ProductCartItems" };

        public Type Type => typeof(EnovaCart);

        public bool FlattenMapping => false;
        public object GetEnovaProperty(BaseObject obj, string propertyName, List<EnovaLanguage> mappingLanguages)
        {
            var cart = (EnovaCart)obj;

            var cartItems = cart.GetCartItems<EnovaProductCartItem>().Select(x => new Dictionary<string, object>()
                {
                    {"ID", x.ID },
                    {"ProductID", x.Product?.ID ?? 0 },
                    {"Identifier", x.Identifier },
                    {"ProductIdentifier", x.ProductIdentifier },
                    {"PriceExclTax", x.GetPrice(false) },
                    {"PriceInclTax", x.GetPrice(true) },
                    {"Quantity", x.Quantity },
                }.MapLanguageProperty("Name", mappingLanguages, x.GetName));
            return cartItems;
        }

        public void SetEnovaProperty(BaseObject obj, string propertyName, object value, IDictionary<string, object> otherValues)
        {
            if(value == null)
                return;

            var context = obj.GetContext();
            var cart = (EnovaCart)obj;
            var productRows = JsonConvert.DeserializeAnonymousType(value.ToString(), new[] { new { ID = 0, ProductIdentifier = "", Quantity = 0d, MarkForDelete = false } });

            foreach (var productRow in productRows)
            {
                var item = cart.GetCartItems<EnovaProductCartItem>().FirstOrDefault(x => x.ProductIdentifier == productRow.ProductIdentifier);
                if (productRow.MarkForDelete)
                {
                    if(item != null)
                        cart.DeleteCartItem(item);
                    continue;
                }

                if (item == null)
                {
                    item = EnovaObjectCreationHelper.CreateNew<EnovaProductCartItem>(context);
                    cart.AddCartItem(item);
                    item.Product = EnovaBaseProduct.Find(context, productRow.ProductIdentifier);
                }

                item.Quantity = productRow.Quantity > 0 ? productRow.Quantity : 1;
            }
        }
    }
}
