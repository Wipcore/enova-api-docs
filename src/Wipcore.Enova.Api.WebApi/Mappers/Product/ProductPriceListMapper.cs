using System;
using System.Collections.Generic;
using System.Linq;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Abstractions.Interfaces;
using Wipcore.Enova.Core;
using Wipcore.Enova.Generics;

namespace Wipcore.eNova.Api.WebApi.Mappers.Product
{
    public class ProductPriceListMapper : IPropertyMapper
    {
        public bool PostSaveSet => false;
        public bool InheritMapper => true;
        public bool FlattenMapping => false;

        public int Priority => 0;
        public MapType MapType => MapType.MapFromEnovaAllowed;

        public List<string> Names => new List<string>() { "Prices" };

        public Type Type => typeof(EnovaBaseProduct);


        public object GetEnovaProperty(BaseObject obj, string propertyName)
        {
            var product = (EnovaBaseProduct)obj;
            var priceLists = product.GetPriceLists<EnovaPriceList>();
            var prices = new List<object>();

            foreach (var priceList in priceLists)
            {
                var accessGroups = priceList.GetObjectsWithSpecificAccess(product.GetContext(), typeof(CustomerGroup)).Cast<CustomerGroup>();
                var price = new
                {
                    PriceListId = priceList.ID,
                    PriceListIdentifier = priceList.Identifier,
                    PriceExclTax = priceList.GetPrice(product, false),
                    PriceInclTax = priceList.GetPrice(product, true),
                    GroupsWithAccessToPrice = accessGroups.Select(x => new
                    {
                        Identifier = x.Identifier,
                        Name = x.Name
                    }),
                    Currency = priceList.Currency?.Identifier
                };
                prices.Add(price);
            }

            return prices;
        }

        public void SetEnovaProperty(BaseObject obj, string propertyName, object value, IDictionary<string, object> otherValues)
        {
            throw new NotImplementedException();
        }
    }
}
