using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Wipcore.Core.SessionObjects;
using Wipcore.eNova.Api.WebApi.Helpers;
using Wipcore.Enova.Api.Interfaces;
using Wipcore.Enova.Api.WebApi.Helpers;
using Wipcore.Enova.Core;

namespace Wipcore.eNova.Api.WebApi.Mappers.PriceList
{
    /// <summary>
    /// Maps all prices on a pricelist, with and without tax.
    /// </summary>
    public class PriceListPriceMapper : IPropertyMapper
    {
        public bool PostSaveSet => false;
        public List<string> Names => new List<string>() {"PricesInclTax","PricesExclTax", "ProductPrices" };
        public Type Type => typeof (EnovaPriceList);
        public bool InheritMapper => true;
        public int Priority => 0;
        public MapType MapType => MapType.MapFromAndToEnovaAllowed;

        public void SetEnovaProperty(BaseObject obj, string propertyName, object value, IDictionary<string, object> otherValues)
        {
            if (value == null)
                return;

            var pricelist = (EnovaPriceList)obj;
            var context = obj.GetContext();
            if (String.Equals(propertyName, "ProductPrices", StringComparison.InvariantCultureIgnoreCase))
            {
                foreach (var i in value as dynamic)
                {
                    var item = JsonConvert.DeserializeAnonymousType(i.ToString(), new { Identifier = "", PriceExclTax = 0m, MarkForDelete = false});
                    var productIdentifier = item.Identifier;
                    var price = item.PriceExclTax;
                    var product = EnovaBaseProduct.Find(context, productIdentifier);

                    if(item.MarkForDelete)
                        pricelist.RemoveProduct(product);
                    else
                        pricelist.SetPrice(product, price);
                }
            }
            else
            {
                var includeTax = String.Equals(propertyName, "PricesInclTax", StringComparison.InvariantCultureIgnoreCase);
                var tax = context?.CurrentTaxationRule?.DefaultTax;
                var dictionary = (Dictionary<string, decimal>) value;
                foreach (var item in dictionary)
                {
                    var product = EnovaBaseProduct.Find(context, item.Key);
                    var price = includeTax ? PriceHelper.RemoveTax(item.Value, tax) : item.Value;
                    pricelist.SetPrice(product, price);
                }
            }
        }

        public object GetEnovaProperty(BaseObject obj, string propertyName)
        {
            var pricelist = (EnovaPriceList) obj;
            var products = pricelist.GetProducts(typeof (EnovaBaseProduct)).Cast<EnovaBaseProduct>();

            if (String.Equals(propertyName, "ProductPrices", StringComparison.InvariantCultureIgnoreCase))//detailed info
            {
                return products.Select(x => new
                {
                    ID = x.ID,
                    Identifier = x.Identifier,
                    Name = x.Name,
                    PriceInclTax = pricelist.GetPrice(x, true),
                    PriceExclTax = pricelist.GetPrice(x, false),
                });
            }
            else //only price info
            {
                var includeTax = String.Equals(propertyName, "PricesInclTax", StringComparison.InvariantCultureIgnoreCase);
                var prices = new Dictionary<string, decimal>();
                foreach (var product in products)
                {
                    var price = pricelist.GetPrice(product, includeTax);
                    prices.Add(product.Identifier, price);
                }

                return prices;
            }
            
        }
    }
}
