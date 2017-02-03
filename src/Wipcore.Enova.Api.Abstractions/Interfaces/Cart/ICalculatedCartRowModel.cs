namespace Wipcore.Enova.Api.Abstractions.Interfaces.Cart
{
    /// <summary>
    /// Model for a cart/order row with it's prices calculated.
    /// </summary>
    public interface ICalculatedCartRowModel : ICartRowModel
    {
        decimal PriceInclTax { get; set; }

        decimal PriceExclTax { get; set; }

        string Name { get; set; }
    }
}
