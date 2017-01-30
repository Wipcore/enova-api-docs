using System;
using System.Collections.Generic;
using System.Linq;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Abstractions.Interfaces;
using Wipcore.Enova.Core;

namespace Wipcore.Enova.Api.WebApi.Mappers.Product
{
    /// <summary>
    /// Maps active promos for a product.
    /// </summary>
    public class ProductPromoMapper : IPropertyMapper
    {
        public bool PostSaveSet => false;
        public List<string> Names => new List<string>() {"Promos" };
        public Type Type => typeof (EnovaBaseProduct);
        public bool InheritMapper => true;
        public bool FlattenMapping => false;
        public int Priority => 0;
        public MapType MapType => MapType.MapFromEnovaAllowed;

        public void SetEnovaProperty(BaseObject obj, string propertyName, object value, IDictionary<string, object> otherValues)
        {
            throw new NotImplementedException();
        }

        public object GetEnovaProperty(BaseObject obj, string propertyName)
        {
            var product = (EnovaBaseProduct)obj;
            var promos = product.GetPromos(typeof (EnovaPromo)).Cast<EnovaPromo>().ToList();

            if(!promos.Any())
                return null;

            var mappedPromos = promos.Select(promo => new Dictionary<string, object>() {{"Identifier", promo.Identifier }, {"Name", promo.Name}});

            return mappedPromos;
        }
    }
}
