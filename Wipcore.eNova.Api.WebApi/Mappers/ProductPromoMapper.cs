using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Interfaces;
using Wipcore.Enova.Core;
using Wipcore.Enova.Generics;

namespace Wipcore.eNova.Api.WebApi.Mappers
{
    public class ProductPromoMapper : IPropertyMapper
    {
        public List<string> Names => new List<string>() {"Promos" };
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
            var product = (EnovaBaseProduct)obj;
            var promos = product.GetPromos(typeof (EnovaPromo)).Cast<EnovaPromo>().ToList();

            if(!promos.Any())
                return null;

            var mappedPromos = promos.Select(promo => new Dictionary<string, object>() {{"Identifier", promo.Identifier }, {"Name", promo.Name}});

            return mappedPromos;
        }
    }
}
