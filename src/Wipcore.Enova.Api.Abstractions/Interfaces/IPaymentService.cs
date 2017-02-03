namespace Wipcore.Enova.Api.Abstractions.Interfaces
{
    public interface IPaymentService
    {
        /// <summary>
        /// Saves a payment from given model.
        /// </summary>
        IPaymentModel SavePayment(IPaymentModel payment);
    }
}