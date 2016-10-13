using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Interfaces;
using Wipcore.Enova.Core;

namespace Wipcore.eNova.Api.WebApi.Mappers.Promo
{
    public class PromoResultMapper : IPropertyMapper
    {
        public List<string> Names => new List<string>() { "Results" };
        public Type Type => typeof(EnovaPromo);
        public bool InheritMapper => true;

        public int Priority => 0;
        public MapType MapType => MapType.MapFromEnovaAllowed;

        public object MapFromEnovaProperty(BaseObject obj, string propertyName)
        {
            var promo = (EnovaPromo) obj; //TODO remake. match results to conditions, otherwise makes no sense in presentation
            var results = new List<object>();
            foreach (var enovaItem in promo.GetResults())
            {
                var decimals = 0;
                if (enovaItem is EnovaFixedPricePromoResult)
                {
                    var item = (EnovaFixedPricePromoResult)enovaItem;
                    results.Add(new
                    {
                        ID = item.ID,
                        Price = item.GetPrice(out decimals),
                        TaxIncluded = item.TaxIncluded
                    });
                }
                else if (enovaItem is EnovaFreeProductPromoResult)
                {
                    var item = (EnovaFreeProductPromoResult)enovaItem;
                    results.Add(new
                    {
                        ID = item.ID,
                        ProductID = item.Product.ID,
                        Quantity = item.Quantity
                    });
                }
                else if (enovaItem is EnovaDiscountedProductPromoResult)
                {
                    var item = (EnovaDiscountedProductPromoResult)enovaItem;
                    results.Add(new
                    {
                        ID = item.ID,
                        Price = item.GetPrice(out decimals),
                        Quantity = item.MaxQuantity,
                        ProductID = item.Product.ID
                    });
                }
                else if (enovaItem is EnovaProductQuantityPromoResult)
                {
                    var item = (EnovaProductQuantityPromoResult)enovaItem;
                    results.Add(new
                    {
                        ID = item.ID,
                        Quantity = item.Quantity
                    });
                }
            }

            return results;
        }

        public void MapToEnovaProperty(BaseObject obj, string propertyName, object value, IDictionary<string, object> otherValues)
        {
            throw new NotImplementedException();
        }
    }
}
