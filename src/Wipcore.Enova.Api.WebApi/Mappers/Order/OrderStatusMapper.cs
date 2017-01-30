using System;
using System.Collections.Generic;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Abstractions.Interfaces;
using Wipcore.Enova.Core;
using Wipcore.Enova.Generics;

namespace Wipcore.eNova.Api.WebApi.Mappers.Order
{
    public class OrderStatusMapper : IPropertyMapper
    {
        public List<string> Names => new List<string>() { "ShippingStatus" };
        public Type CmoType => typeof(CmoEnovaOrder);
        public Type Type => typeof(EnovaOrder);
        public bool InheritMapper => true;
        public bool FlattenMapping => false;
        public int Priority => 0;
        public MapType MapType => MapType.MapFromAndToEnovaAllowed;

        public bool PostSaveSet => true;
        
        public object GetEnovaProperty(BaseObject obj, string propertyName)
        {
            var order = (EnovaOrder)obj;
            return order.ShippingStatus?.Identifier;
        }

        public void SetEnovaProperty(BaseObject obj, string propertyName, object value, IDictionary<string, object> otherValues)
        {
            if (value == null)
                return;

            var order = (EnovaOrder)obj;
            var shippingStatus = order.GetContext().FindObject<EnovaShippingStatus>(value.ToString());
            order.ChangeShippingStatus(shippingStatus, null);//TODO maybe insert paymenthandlers by configuration here
        }
    }
}
