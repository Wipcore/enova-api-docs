using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Interfaces;
using Wipcore.Enova.Core;
using Wipcore.Enova.Generics;

namespace Wipcore.eNova.Api.WebApi.Mappers.Order
{
    public class PaymentOrderItemMapper : IPropertyMapper
    {
        public bool InheritMapper => true;

        public int Priority => 0;
        public MapType MapType => MapType.MapFromEnovaAllowed;

        public List<string> Names => new List<string>() { "PaymentInfo" };

        public Type Type => typeof(EnovaOrder);

        public object MapFromEnovaProperty(BaseObject obj, string propertyName)
        {
            Currency currency;
            var order = (EnovaOrder)obj;

            var orderItems =    from item in order.GetOrderItems<EnovaPaymentTypeOrderItem>()
                                let payment = obj.GetContext().FindObject<EnovaPayment>(item.PaymentId)
                                select new 
                                {
                                    ID = item.ID,
                                    Identifier = item.Identifier,
                                    Name = item.Name,
                                    PaymentIdentifier = item.PaymentType?.Identifier,
                                    Amount = item.GetAmount(out currency),
                                    PaymentDate = payment?.PaymentDate.ToString(),
                                    Paid = payment != null && item.Paid
                                };
                                

            return orderItems.FirstOrDefault();
        }

        public void MapToEnovaProperty(BaseObject obj, string propertyName, object value, IDictionary<string, object> otherValues)
        {
            throw new NotImplementedException();
        }
    }
}
