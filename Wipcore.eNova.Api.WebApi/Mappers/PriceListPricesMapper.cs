using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Interfaces;
using Wipcore.Enova.Core;

namespace Wipcore.eNova.Api.WebApi.Mappers
{
    /// <summary>
    /// Maps all prices on a pricelist, with and without tax.
    /// </summary>
    public class PriceListPriceMapper : IPropertyMapper
    {
        public List<string> Names => new List<string>() {"PricesInclTax","PricesExclTax" };
        public Type Type => typeof (EnovaPriceList);
        public bool InheritMapper => true;
        public int Priority => 0;
        public MapType MapType => MapType.MapFrom;

        public object MapToEnovaProperty(BaseObject obj, string propertyName)
        {
            throw new NotImplementedException();
        }

        public object MapFromEnovaProperty(BaseObject obj, string propertyName)
        {
            var includeTax = String.Equals(propertyName, "PricesInclTax", StringComparison.InvariantCultureIgnoreCase);
            var prices = new Dictionary<string, decimal>();
            var pricelist = (EnovaPriceList) obj;
            var products = pricelist.GetProducts(typeof (EnovaBaseProduct)).Cast<EnovaBaseProduct>();
            foreach (var product in products)
            {
                var price = pricelist.GetPrice(product, includeTax);
                prices.Add(product.Identifier, price);
            }

            return prices;
        }
    }
}
