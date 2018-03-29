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
    public class PaymentCartItemMapper : IPropertyMapper
    {
        private readonly IConfigService _configService;

        public PaymentCartItemMapper(IConfigService configService)
        {
            _configService = configService;
        }


        public bool PostSaveSet => false;
        public bool InheritMapper => true;

        public int Priority => 0;
        public MapType MapType => MapType.MapFromAndToEnovaAllowed;

        public List<string> Names => new List<string>() { "PaymentCartItem", "NewPaymentType" };

        public Type Type => typeof(EnovaCart);
        public bool FlattenMapping => false;

        public object GetEnovaProperty(BaseObject obj, string propertyName, List<EnovaLanguage> mappingLanguages)
        {
            if (propertyName.Equals("NewPaymentType", StringComparison.InvariantCultureIgnoreCase))
                return "";

            var cart = (EnovaCart)obj;
            var context = obj.GetContext();
            var paymentItem = cart.GetCartItems<EnovaPaymentTypeCartItem>().FirstOrDefault();

            if (paymentItem == null)
                return null;

            return new Dictionary<string, object>()
            {
                {"ID", paymentItem.ID},
                {"PaymentID", paymentItem.PaymentType?.ID ?? 0},
                {"Identifier", paymentItem.Identifier},
                {"PaymentIdentifier", paymentItem.PaymentType?.Identifier ?? String.Empty},
                {"PriceExlTax", paymentItem.GetPrice(false)},
                {"PriceInclTax", paymentItem.GetPrice(true)},
                {"PriceExclTaxString", context.AmountToString(paymentItem.GetPrice(false), context.CurrentCurrency, _configService.DecimalsInAmountString())},
                {"PriceInclTaxString", context.AmountToString(paymentItem.GetPrice(true), context.CurrentCurrency, _configService.DecimalsInAmountString())},
            }.MapLanguageProperty("Name", mappingLanguages, obj.GetName);
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
                cartItem = EnovaObjectMakerHelper.CreateNew<EnovaPaymentTypeCartItem>(context);
                cart.AddCartItem(cartItem);
            }

            if (cartItem.PaymentType?.Identifier != paymentIdentifier)
            {
                cartItem.PaymentType = EnovaPaymentType.Find(context, paymentIdentifier);
            }

        }
    }
}
