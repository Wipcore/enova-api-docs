using System;

namespace Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.Order
{
    public class OrderMiniModel
    {
        public int OrderId { get; set; }

        public string OrderIdentifier { get; set; }

        public double NumberOfItems { get; set; }

        public decimal TotalPriceInclTax { get; set; }

        public string ShippingStatus { get; set; }

        public DateTime OrderDate { get; set; }
    }
}
