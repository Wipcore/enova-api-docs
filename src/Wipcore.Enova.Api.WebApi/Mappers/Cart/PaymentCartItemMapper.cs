using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Interfaces;
using Wipcore.Enova.Api.WebApi.Helpers;
using Wipcore.Enova.Core;
using Wipcore.Enova.Generics;

namespace Wipcore.eNova.Api.WebApi.Mappers.Cart
{
    public class PaymentCartItemMapper : IPropertyMapper
    {
        public bool PostSaveSet => false;
        public bool InheritMapper => true;

        public int Priority => 0;
        public MapType MapType => MapType.MapFromAndToEnovaAllowed;

        public List<string> Names => new List<string>() { "PaymentCartItem", "NewPaymentType" };

        public Type Type => typeof(EnovaCart);

        public object GetEnovaProperty(BaseObject obj, string propertyName)
        {
            if (propertyName.Equals("NewPaymentType", StringComparison.InvariantCultureIgnoreCase))
                return "";

            var cart = (EnovaCart)obj;
            var shippingItem = cart.GetCartItems<EnovaPaymentTypeCartItem>();

            return shippingItem.Select(x => new
            {
                x.ID,
                x.Identifier,
                x.Name,
                PaymentIdentifier = x.PaymentType?.Identifier,
                PriceExlTax = x.GetPrice(false),
                PriceInclTax = x.GetPrice(true),
            }).FirstOrDefault();
        }

        public void SetEnovaProperty(BaseObject obj, string propertyName, object value, IDictionary<string, object> otherValues)
        {
            var cart = (EnovaCart)obj;
            var context = cart.GetContext();
            var cartItem = cart.GetCartItems<EnovaPaymentTypeCartItem>().FirstOrDefault();

            if (propertyName.Equals("PaymentCartItem", StringComparison.InvariantCultureIgnoreCase))
            {
                if (value != null)
                {
                    var paymentModel = JsonConvert.DeserializeAnonymousType(value.ToString(), new { MarkForDelete = false });
                    if (paymentModel.MarkForDelete)
                    {
                        if (cartItem != null)
                            cart.DeleteCartItem(cartItem);
                    }
                }
                return;
            }

            var paymentIdentifier = value?.ToString();
            if (String.IsNullOrEmpty(paymentIdentifier))
                return;

            if (cartItem == null)
            {
                cartItem = EnovaObjectCreationHelper.CreateNew<EnovaPaymentTypeCartItem>(context);
                cart.AddCartItem(cartItem);
            }

            if (cartItem.PaymentType?.Identifier != paymentIdentifier)
            {
                cartItem.PaymentType = EnovaPaymentType.Find(context, paymentIdentifier);
            }

        }
    }
}
