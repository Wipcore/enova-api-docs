using System;
using System.Collections.Generic;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Interfaces;
using Wipcore.Enova.Core;

namespace Wipcore.eNova.Api.WebApi.Mappers.Product
{
    public class ProductPriceExclTaxMapper : IPropertyMapper
    {
        public bool InheritMapper => true;

        public int Priority => 0;
        public MapType MapType => MapType.MapFrom;

        public List<string> Names => new List<string>(){"PriceExclTax", "PriceInclTax"};


        public Type Type => typeof(EnovaBaseProduct);
        

        public object MapFromEnovaProperty(BaseObject obj, string propertyName)
        {
            var includeTax = String.Equals(propertyName, "PriceInclTax", StringComparison.InvariantCultureIgnoreCase);
            var product = (EnovaBaseProduct)obj;
            var price = product.GetPrice(includeTax);
            return price;
        }

        public object MapToEnovaProperty(BaseObject obj, string propertyName)
        {
            throw new NotImplementedException();
        }
    }
}
