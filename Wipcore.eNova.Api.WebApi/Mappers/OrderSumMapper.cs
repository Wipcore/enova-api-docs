using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Fasterflect;
using Wipcore.Core;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Interfaces;
using Wipcore.Enova.Api.WebApi;
using Wipcore.Enova.Core;

namespace Wipcore.eNova.Api.WebApi.Mappers
{
    /// <summary>
    /// Maps the order sum for a order, with and without tax.
    /// </summary>
    public class OrderSumMapper : IPropertyMapper, ICmoProperty
    {
        private readonly IContextService _contextService;

        public OrderSumMapper(IContextService contextService)
        {
            _contextService = contextService;
        }

        public List<string> Names => new List<string>() { "SumInclTax", "SumExclTax" };
        public Type CmoType => typeof (CmoEnovaOrder);
        public Type Type => typeof (EnovaOrder);
        public bool InheritMapper => true;

        public int Priority => 0;
        public MapType MapType => MapType.MapFrom;

        public void MapToEnovaProperty(BaseObject obj, string propertyName, object value, IDictionary<string, object> otherValues)
        {
            throw new NotImplementedException();
        }

        public object MapFromEnovaProperty(BaseObject obj, string propertyName)
        {
            var order = (EnovaOrder) obj;
            decimal taxAmount;
            int decimals;
            var currency = obj.GetContext()?.CurrentCurrency;
            double currencyFactor;
            var sum = order.GetSum(out taxAmount, out decimals, ref currency, out currencyFactor);

            if (String.Equals(propertyName, "SumExclTax", StringComparison.InvariantCultureIgnoreCase))
                return sum - taxAmount;

            return sum;
        }

        public object GetProperty(CmoDbObject obj, CmoContext cmoContext, string propertyName, CmoLanguage language)
        {
            int decimals;
            var context = _contextService.GetContext(); 
            var currency = (CmoCurrency)context.CurrentCurrency?.ActiveCmoObject;
            if (cmoContext == null)
                cmoContext = context.GetCmoContext();
            var order = (CmoEnovaOrder) obj;
            var includeTax = String.Equals(propertyName, "SumExclTax", StringComparison.InvariantCultureIgnoreCase);

            var sum = order.GetSum(cmoContext, out decimals, ref currency, includeTax, true);
            return sum;
        }
    }
}
