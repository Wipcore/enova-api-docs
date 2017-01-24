using System;
using System.Collections.Generic;
using Wipcore.Core;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Interfaces;
using Wipcore.Enova.Core;

namespace Wipcore.Enova.Api.WebApi.Mappers.Product
{
    /// <summary>
    /// Maps prices with and without tax for a product.
    /// </summary>
    public class ProductPriceExclTaxMapper : IPropertyMapper, ICmoProperty
    {
        public bool PostSaveSet => false;
        public bool InheritMapper => true;
        public bool FlattenMapping => false;

        public int Priority => 0;
        public MapType MapType => MapType.MapFromEnovaAllowed;
        
        public List<string> Names => new List<string>(){"PriceExclTax", "PriceInclTax"};

        public Type Type => typeof(EnovaBaseProduct);

        public Type CmoType => typeof(CmoEnovaBaseProduct);

        public object GetEnovaProperty(BaseObject obj, string propertyName)
        {
            var includeTax = String.Equals(propertyName, "PriceInclTax", StringComparison.InvariantCultureIgnoreCase);
            var product = (EnovaBaseProduct)obj;
            var price = product.GetPrice(includeTax);
            return price;
        }

        public object GetProperty(CmoDbObject obj, CmoContext context, string propertyName, CmoLanguage language)
        {
            var includeTax = String.Equals(propertyName, "PriceInclTax", StringComparison.InvariantCultureIgnoreCase);
            var product = (CmoEnovaBaseProduct)obj;
            int decimals;
            var wipPrice = product.GetPrice(context, 1, out decimals, null, includeTax, true, DateTime.Now);
            var price = includeTax ? wipPrice.Amount + wipPrice.CalculateTaxAmount() : wipPrice.Amount;
            return price;
        }

        public void SetEnovaProperty(BaseObject obj, string propertyName, object value, IDictionary<string, object> otherValues)
        {
            throw new NotImplementedException();
        }
    }
}
