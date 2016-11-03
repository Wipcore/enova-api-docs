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
        public bool PostSaveSet => false;

        public object GetEnovaProperty(BaseObject obj, string propertyName)
        {
            var order = (EnovaOrder) obj;
            if (propertyName.Equals("Currency", StringComparison.InvariantCultureIgnoreCase))
                return order.Currency?.Identifier;

            return order.Country?.Identifier;
        }

        public void SetEnovaProperty(BaseObject obj, string propertyName, object value, IDictionary<string, object> otherValues)
        {
            var order = (EnovaOrder)obj;
            if (propertyName.Equals("Currency", StringComparison.InvariantCultureIgnoreCase))
            { /*don't set */}
            else
                order.Country = value == null ? null : EnovaCountry.Find(order.GetContext(), value.ToString());
        }
    }
}
