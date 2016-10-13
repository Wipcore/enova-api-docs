using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Core;

namespace Wipcore.eNova.Api.WebApi.Helpers
{
    public class PriceHelper
    {
        public static decimal TotalPriceInclTax(EnovaOrder order)
        {
            return TotalPrice(order, true);
        }
        public static decimal TotalPriceExclTax(EnovaOrder order)
        {
            return TotalPrice(order, false);
        }
        private static decimal TotalPrice(EnovaOrder order, bool includeTax)
        {
            decimal taxAmount;
            int decimals;
            Currency currency = null;
            double currenyFactor;
            var price = order.GetSum(out taxAmount, out decimals, ref currency, out currenyFactor);

            return includeTax ? price + taxAmount : price;
        }

        public static decimal RemoveTax(decimal amount, Tax tax)
        {
            if (tax == null || tax.Rate <= 0)
                return amount;

            var rate = (decimal)tax.Rate / 100.0M;
            return amount / (rate + Decimal.One);
        }
    }
}
