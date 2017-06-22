using System;
using System.Collections.Generic;
using System.Runtime.Caching;
using Wipcore.Core;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Abstractions.Interfaces;
using Wipcore.Enova.Core;
using Wipcore.Enova.Generics;
using System.Linq;
using Wipcore.Enova.Api.Abstractions.Internal;

namespace Wipcore.Enova.Api.WebApi.Mappers.Product
{
    /// <summary>
    /// This mapper maps list prices taken from default price list, and IsDiscounted property which is true if listprice is less than current price of product.
    /// </summary>
    public class ProductDiscountedMapper : IPropertyMapper, ICmoProperty
    {
        public bool PostSaveSet => false;
        private readonly ObjectCache _cache;
        private readonly IContextService _contextService;

        public ProductDiscountedMapper(ObjectCache cache, IContextService contextService)
        {
            _cache = cache;
            _contextService = contextService;
        }

        public List<string> Names  => new List<string>() {"IsDiscounted", "ListPriceExclTax", "ListPriceInclTax"};
        public Type CmoType => typeof (CmoEnovaBaseProduct);
        public Type Type => typeof (EnovaBaseProduct);
        public bool InheritMapper => true;
        public bool FlattenMapping => false;

        public int Priority => 0;
        public MapType MapType => MapType.MapFromEnovaAllowed;

        
        public object GetEnovaProperty(BaseObject obj, string propertyName, List<EnovaLanguage> mappingLanguages)
        {
            var defaultPriceList = GetDefaultPriceList();
            var product = (EnovaBaseProduct)obj;
            var returnIsDiscounted = String.Equals(propertyName, "IsDiscounted", StringComparison.InvariantCultureIgnoreCase);

            if (defaultPriceList == null || !defaultPriceList.HasProduct(product))
                return returnIsDiscounted ? (object)false : 0m;

            var includeTax = String.Equals(propertyName, "ListPriceInclTax", StringComparison.InvariantCultureIgnoreCase);
            var currentPrice = product.GetPrice(includeTax);
            var defaultPrice = defaultPriceList.GetPrice(product, includeTax);

            if (String.Equals(propertyName, "IsDiscounted", StringComparison.InvariantCultureIgnoreCase))
                return currentPrice < defaultPrice;

            return defaultPrice;
        }

        

        public object GetProperty(CmoDbObject obj, CmoContext context, string propertyName, CmoLanguage language)
        {
            var product = (CmoEnovaBaseProduct)obj;
            var defaultPriceList = GetDefaultPriceList().ActiveCmoObject as CmoEnovaPriceList;
            var returnIsDiscounted = String.Equals(propertyName, "IsDiscounted", StringComparison.InvariantCultureIgnoreCase);

            if (defaultPriceList == null || !defaultPriceList.HasProduct(context, product))
                return returnIsDiscounted ? (object)false : 0m;

            if (context == null)
                context = _contextService.GetContext().GetCmoContext();

            int decimals;
            bool quantityIsValid;
            CmoCurrency currency = null;
            var includeTax = String.Equals(propertyName, "ListPriceInclTax", StringComparison.InvariantCultureIgnoreCase);
            var currentWipPrice = product.GetPrice(context, 1, out decimals, null, includeTax, true, DateTime.Now);
            var currentPrice = includeTax ? currentWipPrice.Amount + currentWipPrice.CalculateTaxAmount() : currentWipPrice.Amount; 
            var defaultPrice = product.GetPrice(context, defaultPriceList, 1, includeTax, ref currency, out quantityIsValid);

            if (returnIsDiscounted)
                return currentPrice < defaultPrice;

            return defaultPrice;
        }

        public void SetEnovaProperty(BaseObject obj, string propertyName, object value, IDictionary<string, object> otherValues)
        {
            throw new NotImplementedException();
        }

        private EnovaPriceList GetDefaultPriceList()
        {
            const string key = "api_defaultpricelist";
            var value = _cache.Get(key);

            if (value != null)
                return value as EnovaPriceList;

            var context = _contextService.GetContext();
            var currentCurrencyId = context.CurrentCurrency?.ID ?? -1;
            var defaultPriceList =
                context.Search<EnovaPriceList>("IsDefault = true AND CurrencyID = " + currentCurrencyId).FirstOrDefault();

            _cache.Add(key, (object)defaultPriceList ?? "", DateTime.Now.AddMinutes(10));
            return defaultPriceList;
        }
    }
}
