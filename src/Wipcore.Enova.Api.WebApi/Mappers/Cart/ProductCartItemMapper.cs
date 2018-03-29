using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Abstractions.Interfaces;
using Wipcore.Enova.Api.Abstractions.Internal;
using Wipcore.Enova.Api.WebApi.Helpers;
using Wipcore.Enova.Core;
using Wipcore.Enova.Generics;

namespace Wipcore.eNova.Api.WebApi.Mappers.Cart
{
    public class ProductCartItemMapper : IPropertyMapper
    {
        private readonly IConfigService _configService;

        public ProductCartItemMapper(IConfigService configService)
        {
            _configService = configService;
        }

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
            var context = obj.GetContext();

            var cartItems = from x in cart.GetCartItems<EnovaProductCartItem>()
                             let priceExclTax = x.GetPrice(false)
                             let priceInclTax = x.GetPrice(true)
                             select new Dictionary<string, object>() {
                                {"ID", x.ID},
                                {"ProductID", x.Product?.ID ?? 0},
                                {"Identifier", x.Identifier},
                                {"ProductIdentifier", x.ProductIdentifier},
                                {"PriceExclTax", priceExclTax},
                                {"PriceInclTax", priceInclTax},
                                {"PriceExclTaxString", context.AmountToString(priceExclTax, context.CurrentCurrency, _configService.DecimalsInAmountString())},
                                {"PriceInclTaxString", context.AmountToString(priceInclTax, context.CurrentCurrency, _configService.DecimalsInAmountString())},
                                {"TotalPriceExclTaxString", context.AmountToString(priceExclTax * (decimal)x.Quantity, context.CurrentCurrency, _configService.DecimalsInAmountString())},
                                {"TotalPriceInclTaxString", context.AmountToString(priceInclTax * (decimal)x.Quantity, context.CurrentCurrency, _configService.DecimalsInAmountString())},
                                {"Quantity", x.Quantity},
                                {"Comment", x.Comment }
                            }.MapLanguageProperty("Name", mappingLanguages, x.GetName);
            
            return cartItems;
        }

        public void SetEnovaProperty(BaseObject obj, string propertyName, object value, IDictionary<string, object> otherValues)
        {
            if(value == null)
                return;

            var context = obj.GetContext();
            var cart = (EnovaCart)obj;
            var productRows = JsonConvert.DeserializeAnonymousType(value.ToString(), new[] { new { ID = 0, Identifier = "", ProductIdentifier = "", Quantity = 0d, Comment = "", MarkForDelete = false } });

            foreach (var productRow in productRows)
            {
                var item = !String.IsNullOrEmpty(productRow.Identifier) ? 
                    cart.GetCartItems<EnovaProductCartItem>().FirstOrDefault(x => x.Identifier == productRow.Identifier) : 
                    cart.GetCartItems<EnovaProductCartItem>().FirstOrDefault(x => String.IsNullOrEmpty(x.Identifier) && x.ProductIdentifier == productRow.ProductIdentifier);

                if (productRow.MarkForDelete)
                {
                    if(item != null)
                        cart.DeleteCartItem(item);
                    continue;
                }

                if (item == null)
                {
                    item = EnovaObjectMakerHelper.CreateNew<EnovaProductCartItem>(context);
                    cart.AddCartItem(item);
                    item.Product = EnovaBaseProduct.Find(context, productRow.ProductIdentifier);
                    item.Identifier = productRow.Identifier ?? String.Empty;
                }

                item.Quantity = productRow.Quantity > 0 ? productRow.Quantity : 1;
                item.Comment = productRow.Comment ?? item.Comment;
            }
        }
    }
}
