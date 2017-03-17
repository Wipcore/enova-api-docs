namespace Wipcore.Enova.Api.Abstractions.Interfaces.Cart
{
    /// <summary>
    /// Model for a cart/order that has had it's prices calculated.
    /// </summary>
    public interface ICalculatedCartModel : ICartModel
    {
        decimal TotalPriceInclTax { get; set; }

        decimal TotalPriceExclTax { get; set; }

        string TotalPriceInclTaxString { get; set; }
        string TotalPriceExclTaxString { get; set; }
    }
}
