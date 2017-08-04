using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Wipcore.Core;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Abstractions.Interfaces;
using Wipcore.Enova.Api.Abstractions.Internal;
using Wipcore.Enova.Core;

namespace Wipcore.eNova.Api.WebApi.Mappers.Cart
{
    /// <summary>
    /// Maps the cart sum for a cart, with and without tax.
    /// </summary>
    public class CartSumMapper : IPropertyMapper, ICmoProperty
    {
        private readonly IContextService _contextService;
        private readonly IConfigService _configService;

        public CartSumMapper(IContextService contextService, IConfigService configService)
        {
            _contextService = contextService;
            _configService = configService;
        }

        public bool PostSaveSet => false;
        public bool FlattenMapping => false;
        public List<string> Names => new List<string>() { "TotalPriceExclTax", "TotalPriceInclTax", "TotalPriceInclTaxString", "TotalPriceExclTaxString" };
        public Type CmoType => typeof (CmoEnovaCart);
        public Type Type => typeof (EnovaCart);
        public bool InheritMapper => true;
        public int Priority => 0;
        public MapType MapType => MapType.MapFromEnovaAllowed;

        public void SetEnovaProperty(BaseObject obj, string propertyName, object value, IDictionary<string, object> otherValues)
        {
            throw new NotImplementedException();
        }

        public object GetEnovaProperty(BaseObject obj, string propertyName, List<EnovaLanguage> mappingLanguages)
        {
            var cart = (EnovaCart) obj;
            var context = obj.GetContext();
            decimal taxAmount;
            int decimals;
            Currency currency = null;
            decimal rounding;
            var sum = cart.GetPrice(out taxAmount, out rounding, out decimals, ref currency);

            if (String.Equals(propertyName, "TotalPriceInclTax", StringComparison.InvariantCultureIgnoreCase))
                return sum;
            else if (String.Equals(propertyName, "TotalPriceExclTax", StringComparison.InvariantCultureIgnoreCase))
                return sum - taxAmount;
            else if (String.Equals(propertyName, "TotalPriceInclTaxString", StringComparison.InvariantCultureIgnoreCase))
                return context.AmountToString(sum, currency, _configService.DecimalsInAmountString(), true, true);
            else
                return context.AmountToString(sum - taxAmount, currency, _configService.DecimalsInAmountString(), true, true);
        }

        public object GetProperty(CmoDbObject obj, CmoContext cmoContext, string propertyName, CmoLanguage language)
        {
            int decimals;
            decimal taxAmount;
            decimal rounding;
            var context = _contextService.GetContext();
            var currency = (CmoCurrency)context.CurrentCurrency?.ActiveCmoObject;
            if (cmoContext == null)
                cmoContext = context.GetCmoContext();
            var cart = (CmoEnovaCart)obj;

            var sum = cart.GetTotalPrice(cmoContext, out decimals, out taxAmount, out rounding, currency);

            if (String.Equals(propertyName, "TotalPriceExclTax", StringComparison.InvariantCultureIgnoreCase))
                return sum.Amount - taxAmount;
            return sum.Amount;
        }
    }
}
