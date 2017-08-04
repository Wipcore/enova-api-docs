using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Wipcore.Core;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Abstractions.Interfaces;
using Wipcore.Enova.Api.Abstractions.Internal;
using Wipcore.Enova.Core;

namespace Wipcore.eNova.Api.WebApi.Mappers.Order
{
    /// <summary>
    /// Maps the order sum for a order, with and without tax.
    /// </summary>
    public class OrderSumMapper : IPropertyMapper, ICmoProperty
    {
        private readonly IContextService _contextService;
        private readonly IConfigService _configService;

        public OrderSumMapper(IContextService contextService, IConfigService configService)
        {
            _contextService = contextService;
            _configService = configService;
        }
        public bool FlattenMapping => false;
        public bool PostSaveSet => false;
        public List<string> Names => new List<string>() { "TotalPriceExclTax", "TotalPriceInclTax", "TotalPriceInclTaxString", "TotalPriceExclTaxString" };
        public Type CmoType => typeof (CmoEnovaOrder);
        public Type Type => typeof (EnovaOrder);
        public bool InheritMapper => true;

        public int Priority => 0;
        public MapType MapType => MapType.MapFromEnovaAllowed;
        
        public object GetEnovaProperty(BaseObject obj, string propertyName, List<EnovaLanguage> mappingLanguages)
        {
            var order = (EnovaOrder) obj;
            var context = obj.GetContext();
            decimal taxAmount;
            int decimals;
            Currency currency = null;
            double currencyFactor;
            var sum = order.GetSum(out taxAmount, out decimals, ref currency, out currencyFactor);

            if (String.Equals(propertyName, "TotalPriceInclTax", StringComparison.InvariantCultureIgnoreCase))
                return sum + taxAmount;
            else if (String.Equals(propertyName, "TotalPriceExclTax", StringComparison.InvariantCultureIgnoreCase))
                return sum;
            else if(String.Equals(propertyName, "TotalPriceInclTaxString", StringComparison.InvariantCultureIgnoreCase))
                return context.AmountToString(sum + taxAmount, currency, _configService.DecimalsInAmountString(), true, true);
            else
                return context.AmountToString(sum, currency, _configService.DecimalsInAmountString(), true, true);
        }

        public object GetProperty(CmoDbObject obj, CmoContext cmoContext, string propertyName, CmoLanguage language)
        {
            int decimals;
            var context = _contextService.GetContext(); 
            var currency = (CmoCurrency)context.CurrentCurrency?.ActiveCmoObject;
            if (cmoContext == null)
                cmoContext = context.GetCmoContext();
            var order = (CmoEnovaOrder) obj;
            var includeTax = String.Equals(propertyName, "TotalPriceInclTax", StringComparison.InvariantCultureIgnoreCase);

            var sum = order.GetSum(cmoContext, out decimals, ref currency, includeTax, true);
            return sum;
        }

        public void SetEnovaProperty(BaseObject obj, string propertyName, object value, IDictionary<string, object> otherValues)
        {
            throw new NotImplementedException();
        }
    }
}
