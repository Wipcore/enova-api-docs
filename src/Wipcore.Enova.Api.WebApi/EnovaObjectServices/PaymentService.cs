using System;
using System.Linq;
using System.Net;
using Microsoft.Extensions.Logging;
using Wipcore.Core;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Abstractions.Interfaces;
using Wipcore.Enova.Api.WebApi.Helpers;
using Wipcore.Enova.Core;
using Wipcore.Enova.Generics;

namespace Wipcore.Enova.Api.WebApi.EnovaObjectServices
{
    public class PaymentService : IPaymentService
    {
        private readonly IContextService _contextService;
        private readonly IMappingToEnovaService _mappingToEnovaService;
        private readonly IAuthService _authService;
        private readonly ILogger _logger;


        public PaymentService(IContextService contextService, IMappingToEnovaService mappingToEnovaService, IAuthService authService, ILoggerFactory loggerFactory)
        {
            _contextService = contextService;
            _mappingToEnovaService = mappingToEnovaService;
            _authService = authService;
            _logger = loggerFactory.CreateLogger(GetType().Name);
        }

        /// <summary>
        /// Saves a payment from given model.
        /// </summary>
        public IPaymentModel SavePayment(IPaymentModel payment)
        {
            var context = _contextService.GetContext();
            var enovaPayment = context.FindObject<EnovaPayment>(payment.Identifier) ?? EnovaObjectCreationHelper.CreateNew<EnovaPayment>(context);

            var specifiedOrder = context.FindObject<EnovaOrder>(String.IsNullOrEmpty(payment.Order) ? null : payment.Order);
            var orderOnPayment = context.FindObject<EnovaOrder>(String.IsNullOrEmpty(enovaPayment.RelatedOrderIdentifier) ? null : enovaPayment.RelatedOrderIdentifier);
            
            if (!_authService.AuthorizeUpdate(orderOnPayment?.Customer?.Identifier, specifiedOrder?.Customer?.Identifier))
                throw new HttpException(HttpStatusCode.Unauthorized, "A customer can only update it's own payment.");

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

            _mappingToEnovaService.MapToEnovaObject(enovaPayment, payment.AdditionalValues);

            var newPayment = enovaPayment.ID == default(int);
            enovaPayment.Save();
            _logger.LogInformation("{0} {1} payment with Identifier {2}, Type: {3} and Values: {4}", _authService.LogUser(),
                newPayment ? "Created" : "Updated", enovaPayment.Identifier, enovaPayment.GetType().Name, payment.ToString());

            if (!String.IsNullOrEmpty(payment.Order))
            {
                if(specifiedOrder == null)
                    throw new ObjectNotFoundException(payment.Order);

                //if no paymentorderitem with this payment, create
                if (specifiedOrder.GetOrderItems<EnovaPaymentTypeOrderItem>().All(x => x.PaymentId != enovaPayment.ID))
                {
                    var paymentOrderItem = EnovaObjectCreationHelper.CreateNew<EnovaPaymentTypeOrderItem>(context);
                    paymentOrderItem.PaymentId = enovaPayment.ID;
                    specifiedOrder.AddOrderItem(paymentOrderItem);
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
