using System;
using System.Collections.Generic;
using System.Linq;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Abstractions.Interfaces;
using Wipcore.Enova.Api.Abstractions.Internal;
using Wipcore.Enova.Core;
using Wipcore.Enova.Generics;

namespace Wipcore.eNova.Api.WebApi.Mappers.Order
{
    public class PaymentOrderItemMapper : IPropertyMapper
    {
        private readonly IConfigService _configService;

        public PaymentOrderItemMapper(IConfigService configService)
        {
            _configService = configService;
        }

        public bool PostSaveSet => false;
        public bool InheritMapper => true;
        public bool FlattenMapping => false;
        public int Priority => 0;
        public MapType MapType => MapType.MapFromEnovaAllowed;
        public List<string> Names => new List<string>() { "PaymentOrderItem" };

        public Type Type => typeof(EnovaOrder);

        public object GetEnovaProperty(BaseObject obj, string propertyName, List<EnovaLanguage> mappingLanguages)
        {
            Currency currency;
            var order = (EnovaOrder)obj;
            var context = obj.GetContext();

            var orderItems =    from item in order.GetOrderItems<EnovaPaymentTypeOrderItem>()
                                let payment = obj.GetContext().FindObject<EnovaPayment>(item.PaymentId)
                                select new Dictionary<string, object>()
                                {
                                    { "ID",  item.ID},
                                    { "PaymentID",  item.PaymentType?.ID ?? 0},
                                    { "Identifier", item.Identifier},
                                    { "PaymentIdentifier", item.PaymentType?.Identifier ?? String.Empty},
                                    { "Amount", item.GetAmount(out currency)},
                                    { "AmountString", context.AmountToString(item.GetAmount(out currency), currency, _configService.DecimalsInAmountString())},
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
