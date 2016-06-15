using Wipcore.Enova.Api.Models.Interfaces;

namespace Wipcore.Enova.Api.Interfaces
{
    public interface IPaymentService
    {
        /// <summary>
        /// Saves a payment from given model.
        /// </summary>
        IPaymentModel SavePayment(IPaymentModel payment);
    }
}