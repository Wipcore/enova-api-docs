using System;
using System.Collections.Generic;
using Wipcore.Enova.Api.Models.Interfaces;

namespace Wipcore.Enova.Api.Models
{
    public class PaymentModel : IPaymentModel
    {
        public string Identifier { get; set; }

        public decimal? Amount { get; set; }

        public decimal? CcAuthorizedAmount { get; set; }

        public decimal? CcSettledAmount { get; set; }

        public DateTime? PaymentDate { get; set; }

        public DateTime? CcAuthorizeDate { get; set; }

        public DateTime? CcSettleDate { get; set; }

        public string Order { get; set; }

        public string TypeOfPayment { get; set; }

        public int? Status { get; set; }
        public string Name { get; set; }
        public IDictionary<string, object> AdditionalValues { get; set; }
    }
}
