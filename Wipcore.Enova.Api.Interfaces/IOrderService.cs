namespace Wipcore.Enova.Api.Interfaces
{
    public interface IOrderService
    {
        ICartModel CreateOrder(ICartModel cartModel);
    }
}