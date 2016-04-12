using System;
using System.Linq;
using Wipcore.Core;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Interfaces;
using Wipcore.Enova.Api.Models.Interfaces;
using Wipcore.Enova.Api.WebApi.Helpers;
using Wipcore.Enova.Core;
using Wipcore.Enova.Generics;

namespace Wipcore.eNova.Api.WebApi.EnovaObjectServices
{
    public class PaymentService : IPaymentService
    {
        private readonly IContextService _contextService;
        private readonly IMappingToService _mappingToService;


        public PaymentService(IContextService contextService, IMappingToService mappingToService)
        {
            _contextService = contextService;
            _mappingToService = mappingToService;
        }

        public IPaymentModel SetPayment(IPaymentModel payment)
        {
            var context = _contextService.GetContext();
            var enovaPayment = context.FindObject<EnovaPayment>(payment.Identifier ?? String.Empty)
                               ?? EnovaObjectCreationHelper.CreateNew<EnovaPayment>(context);

            enovaPayment.Edit();
            enovaPayment.Identifier = payment.Identifier ?? String.Empty;
            enovaPayment.Name = payment.Name ?? String.Empty;
            payment.Name = enovaPayment.Name;

            //Set all properties. If model contains them, set model value on enova object.
            //If the model is blank, set model value to enova value instead.

            if (payment.Amount.HasValue)
            {
                enovaPayment.SetAmount(payment.Amount.Value);
            }
            else
            {
                Currency currency;
                payment.Amount = enovaPayment.GetAmount(out currency);
            }

            if (payment.CcAuthorizedAmount.HasValue)
                enovaPayment.CCAuthorizedAmount = payment.CcAuthorizedAmount.Value;
            else
                payment.CcAuthorizedAmount = enovaPayment.CCAuthorizedAmount;

            if (payment.CcSettledAmount.HasValue)
                enovaPayment.CCAuthorizedAmount = payment.CcAuthorizedAmount.Value;
            else
                payment.CcAuthorizedAmount = enovaPayment.CCAuthorizedAmount;

            if (payment.Status.HasValue)
                enovaPayment.Status = payment.Status.Value;
            else
                payment.Status = enovaPayment.Status;

            if (payment.PaymentDate.HasValue)
                enovaPayment.PaymentDate = payment.PaymentDate.Value;
            else
                payment.PaymentDate = enovaPayment.PaymentDate != WipConstants.InvalidDateTime ? enovaPayment.PaymentDate : new DateTime?();

            if (payment.CcAuthorizeDate.HasValue)
                enovaPayment.CCAuthorizeDate = payment.CcAuthorizeDate.Value;
            else
                payment.CcAuthorizeDate = enovaPayment.CCAuthorizeDate != WipConstants.InvalidDateTime ? enovaPayment.CCAuthorizeDate : new DateTime?();

            if (payment.CcSettleDate.HasValue)
                enovaPayment.CCSettleDate = payment.CcSettleDate.Value;
            else
                payment.CcSettleDate = enovaPayment.CCSettleDate != WipConstants.InvalidDateTime ? enovaPayment.CCSettleDate : new DateTime?();

            if (!String.IsNullOrEmpty(payment.TypeOfPayment))
                enovaPayment.TypeOfPayment = (int)Enum.Parse(typeof(EnovaPayment.TypeOfPaymentEnum), payment.TypeOfPayment);
            else
                payment.TypeOfPayment =  ((EnovaPayment.TypeOfPaymentEnum)enovaPayment.TypeOfPayment).ToString();

            enovaPayment.RelatedOrderIdentifier = payment.Order ?? String.Empty;

            payment.AdditionalValues = _mappingToService.MapTo(enovaPayment, payment.AdditionalValues);
            
            enovaPayment.Save();

            if (!String.IsNullOrEmpty(payment.Order))
            {
                
                var order = EnovaOrder.Find(context, payment.Order); //if no paymentorderitem with this payment, create
                if(order.GetOrderItems<EnovaPaymentTypeOrderItem>().All(x => x.PaymentId != enovaPayment.ID))
                {
                    var paymentOrderItem = EnovaObjectCreationHelper.CreateNew<EnovaPaymentTypeOrderItem>(context);
                    paymentOrderItem.PaymentId = enovaPayment.ID;
                    order.AddOrderItem(paymentOrderItem);
                }
            }
            else
            {
                var orderIdentifier = context.Search<EnovaPaymentTypeOrderItem>("PaymentId = " + enovaPayment.ID).FirstOrDefault()?.Order?.Identifier;
                payment.Order = orderIdentifier;
            }

            return payment;
        }
    }
}
