using System;
using System.Collections.Generic;
using System.Linq;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Abstractions.Interfaces;
using Wipcore.Enova.Core;
using Wipcore.Enova.Generics;

namespace Wipcore.eNova.Api.WebApi.Mappers.Order
{
    public class PaymentOrderItemMapper : IPropertyMapper
    {
        public bool PostSaveSet => false;
        public bool InheritMapper => true;
        public bool FlattenMapping => false;
        public int Priority => 0;
        public MapType MapType => MapType.MapFromEnovaAllowed;
        public List<string> Names => new List<string>() { "PaymentInfo" };

        public Type Type => typeof(EnovaOrder);

        public object GetEnovaProperty(BaseObject obj, string propertyName, List<EnovaLanguage> mappingLanguages)
        {
            Currency currency;
            var order = (EnovaOrder)obj;

            var orderItems =    from item in order.GetOrderItems<EnovaPaymentTypeOrderItem>()
                                let payment = obj.GetContext().FindObject<EnovaPayment>(item.PaymentId)
                                select new Dictionary<string, object>()
                                {
                                    { "ID",  item.ID},
                                    { "Identifier", item.Identifier},
                                    { "PaymentIdentifier", item.PaymentType?.Identifier ?? String.Empty},
                                    { "Amount", item.GetAmount(out currency)},
                                    { "PaymentDate", payment?.PaymentDate.ToString() ?? String.Empty},
                                    { "Paid", payment != null && item.Paid}
                                }.MapLanguageProperty("Name", mappingLanguages, item.GetName);
                                

            return orderItems.FirstOrDefault();
        }

        public void SetEnovaProperty(BaseObject obj, string propertyName, object value, IDictionary<string, object> otherValues)
        {
            throw new NotImplementedException();
        }
    }
}
