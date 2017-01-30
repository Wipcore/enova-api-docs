using System;
using System.Collections.Generic;

namespace Wipcore.Enova.Api.Abstractions.Interfaces
{
    public interface IPaymentModel
    {
        string Identifier { get; set; }
        decimal? Amount { get; set; }
        decimal? CcAuthorizedAmount { get; set; }
        decimal? CcSettledAmount { get; set; }
        DateTime? PaymentDate { get; set; }
        DateTime? CcAuthorizeDate { get; set; }
        DateTime? CcSettleDate { get; set; }
        string Order { get; set; }
        string TypeOfPayment { get; set; }
        int? Status { get; set; }
        string Name { get; set; }

        IDictionary<string, object> AdditionalValues { get; set; }
    }
}