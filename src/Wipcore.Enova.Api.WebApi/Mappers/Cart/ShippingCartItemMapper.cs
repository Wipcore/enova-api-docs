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
    public class ShippingCartItemMapper : IPropertyMapper
    {
        public bool PostSaveSet => false;
        public bool InheritMapper => true;

        public int Priority => 0;
        public MapType MapType => MapType.MapFromAndToEnovaAllowed;

        public List<string> Names => new List<string>() { "ShippingCartItem", "NewShippingType" };

        public Type Type => typeof(EnovaCart);
        public bool FlattenMapping => false;

        public object GetEnovaProperty(BaseObject obj, string propertyName, List<EnovaLanguage> mappingLanguages)
        {
            if (propertyName.Equals("NewShippingType", StringComparison.InvariantCultureIgnoreCase))
                return "";

            var cart = (EnovaCart)obj;
            var shippingItem = cart.GetCartItems<EnovaShippingTypeCartItem>().FirstOrDefault();

            if (shippingItem == null)
                return null;

            return new Dictionary<string, object>()
            {
                {"ID", shippingItem.ID},
                {"ShippingID", shippingItem.ShippingType?.ID ?? 0},
                {"Identifier", shippingItem.Identifier},
                {"ShippingIdentifier", shippingItem.ShippingType?.Identifier ?? String.Empty},
                {"PriceExlTax", shippingItem.GetPrice(false)},
                {"PriceInclTax", shippingItem.GetPrice(true)}
            }.MapLanguageProperty("Name", mappingLanguages, obj.GetName);
        }

        public void SetEnovaProperty(BaseObject obj, string propertyName, object value, IDictionary<string, object> otherValues)
        {
            var cart = (EnovaCart)obj;
            var context = cart.GetContext();
            
            var cartItem = cart.GetCartItems<EnovaShippingTypeCartItem>().FirstOrDefault();

            if (propertyName.Equals("ShippingCartItem", StringComparison.InvariantCultureIgnoreCase))
            {
                if (value != null)
                {
                    var shippingModel = JsonConvert.DeserializeAnonymousType(value.ToString(), new { MarkForDelete = false });
                    if (shippingModel.MarkForDelete)
                    {
                        if (cartItem != null)
                            cart.DeleteCartItem(cartItem);
                    }
                }
                return;
            }

            var shippingIdentifier = value?.ToString();
            if(String.IsNullOrEmpty(shippingIdentifier))
                return;

            if (cartItem == null)
            {
                cartItem = EnovaObjectCreationHelper.CreateNew<EnovaShippingTypeCartItem>(context);
                cart.AddCartItem(cartItem);
            }
            
            if(cartItem.ShippingType?.Identifier != shippingIdentifier)
            {
                cartItem.ShippingType = EnovaShippingType.Find(context, shippingIdentifier);
            }
            
        }
    }
}
