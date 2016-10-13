using System;
using System.Collections.Generic;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Interfaces;
using Wipcore.Enova.Core;

namespace Wipcore.eNova.Api.WebApi.Mappers.Order
{
    public class OrderCultureMapper : IPropertyMapper
    {
        public List<string> Names => new List<string>() { "Currency", "Country" };
        public Type Type => typeof(EnovaOrder);
        public bool InheritMapper => true;
        public int Priority => 0;
        public MapType MapType => MapType.MapFromAndToEnovaAllowed;


        public object MapFromEnovaProperty(BaseObject obj, string propertyName)
        {
            var customer = (EnovaOrder) obj;
            if (propertyName.Equals("Currency", StringComparison.InvariantCultureIgnoreCase))
                return customer.Currency?.Identifier;

            return customer.Country?.Identifier;
        }

        public void MapToEnovaProperty(BaseObject obj, string propertyName, object value, IDictionary<string, object> otherValues)
        {
            var customer = (EnovaOrder)obj;
            if (propertyName.Equals("Currency", StringComparison.InvariantCultureIgnoreCase))
            { /*don't set */}
            else
                customer.Country = EnovaCountry.Find(customer.GetContext(), value.ToString());
        }
    }
}
