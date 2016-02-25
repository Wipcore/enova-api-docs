using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Interfaces;
using Wipcore.Enova.Core;
using Wipcore.Enova.Generics;

namespace Wipcore.eNova.Api.WebApi.Mappers
{
    public class ProductDiscountedMapper : IPropertyMapper
    {
        private readonly IConfigurationRoot _configuration;
        

        public ProductDiscountedMapper(IConfigurationRoot configuration)
        {
            _configuration = configuration;
        }

        public List<string> Names  => new List<string>() {"IsDiscounted", "ListPriceExclTax", "ListPriceInclTax"};
        public Type Type => typeof (EnovaBaseProduct);
        public bool InheritMapper => true;
        public int Priority => 0;
        public MapType MapType => MapType.MapFrom;

        public object MapTo(BaseObject obj, string propertyName)
        {
            throw new NotImplementedException();
        }

        public object MapFrom(BaseObject obj, string propertyName)
        {
            var context = obj.GetContext();
            var currentCurrencyIdentifier = context.CurrentCurrency?.Identifier;
            var defaultPriceListIdentifier = _configuration.GetSection("api")?.GetSection("defaultpricelist")?.GetChildren()?.
                FirstOrDefault(x => String.Equals(x.Key, currentCurrencyIdentifier, StringComparison.InvariantCultureIgnoreCase))?.Value;
            var defaultPriceList = context.FindObject<EnovaPriceList>(defaultPriceListIdentifier);
            var product = (EnovaBaseProduct)obj;

            if (defaultPriceList == null || !defaultPriceList.HasProduct(product))
                return null;

            var includeTax = String.Equals(propertyName, "ListPriceInclTax", StringComparison.InvariantCultureIgnoreCase);
            var currentPrice = product.GetPrice(includeTax);
            var defaultPrice = defaultPriceList.GetPrice(product, includeTax);

            if (String.Equals(propertyName, "IsDiscounted", StringComparison.InvariantCultureIgnoreCase))
                return currentPrice < defaultPrice;

            return defaultPrice;
        }
    }
}
