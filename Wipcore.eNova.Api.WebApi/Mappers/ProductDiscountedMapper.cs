using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Wipcore.Core;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Interfaces;
using Wipcore.Enova.Api.WebApi.Helpers;
using Wipcore.Enova.Core;
using Wipcore.Enova.Generics;
using MapType = Wipcore.Enova.Api.Interfaces.MapType;

namespace Wipcore.eNova.Api.WebApi.Mappers
{
    public class ProductDiscountedMapper : IPropertyMapper
    {
        public List<string> Names  => new List<string>() {"IsDiscounted", "ListPriceExclTax", "ListPriceInclTax"};
        public Type Type => typeof (EnovaBaseProduct);
        public bool InheritMapper => true;
        

        public int Priority => 0;
        public MapType MapType => MapType.MapFrom;

        
        public object MapFrom(BaseObject obj, string propertyName)
        {
            var context = obj.GetContext();
            var currentCurrencyId = context.CurrentCurrency?.ID ?? -1;
            var defaultPriceList = context.Search<EnovaPriceList>("IsDefault = true AND CurrencyID = "+currentCurrencyId).FirstOrDefault();
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

        public object MapTo(BaseObject obj, string propertyName)
        {
            throw new NotImplementedException();
        }
    }
}
