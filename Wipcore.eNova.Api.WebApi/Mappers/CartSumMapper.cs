using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Interfaces;
using Wipcore.Enova.Core;

namespace Wipcore.eNova.Api.WebApi.Mappers
{
    public class CartSumMapper : IPropertyMapper
    {
        public List<string> Names => new List<string>() { "SumInclTax", "SumExclTax" };
        public Type Type => typeof (EnovaCart);
        public bool InheritMapper => true;
        public int Priority => 0;
        public MapType MapType => MapType.MapFrom;

        public object MapTo(BaseObject obj, string propertyName)
        {
            throw new NotImplementedException();
        }

        public object MapFrom(BaseObject obj, string propertyName)
        {
            var order = (EnovaCart) obj;
            decimal taxAmount;
            int decimals;
            var currency = obj.GetContext().CurrentCurrency;
            decimal rounding;
            var sum = order.GetPrice(out taxAmount, out rounding, out decimals, ref currency);

            if (String.Equals(propertyName, "SumExclTax", StringComparison.InvariantCultureIgnoreCase))
                return sum - taxAmount;

            return sum;
        }
    }
}
