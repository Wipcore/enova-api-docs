using System.Collections.Generic;
using Newtonsoft.Json;
using Wipcore.Enova.Api.Abstractions.Attributes;

namespace Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.Order
{
    [GroupPresentation("Order info", new string[] { "Identifier", "TotalPriceExclTax", "TotalPriceInclTax", "Currency", "ShippingStatus", "Comment", "CustomerIdentifier" }, sortOrder: 200)]
    [GroupPresentation("Contact info", new string[] { "FirstName", "LastName", "Street", "PostalAddress", "PostalCode", "City", "Country", "Phone", "Email", "RegistrationNumber" }, sortOrder: 300)]
    [GroupPresentation("Order rows", new string[] { "ProductOrderItems", "PromoOrderItems", "ShippingOrderItem", "PaymentOrderItem" }, sortOrder: 400)]
    [GroupPresentation("Order history", new string[] { "ShippingHistory" }, sortOrder: 500)]
    [IndexModel]
    public class OrderModel : BaseModel
    {
        [PropertyPresentation("NumberString", "Customer Id", isEditable: false, isFilterable: true, isGridColumn: true, sortOrder: 300)]
        public int CustomerID { get; set; }

        [PropertyPresentation("CustomerIdentifier", "Customer identifier", isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 300)]
        public string CustomerIdentifier { get; set; }
        
        [PropertyPresentation("String", "First name", isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 200)]
        public string FirstName { get; set; }

        [PropertyPresentation("String", "Last name", isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 200)]
        public string LastName { get; set; }

        [PropertyPresentation("String", null, isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 200)]
        public string Street { get; set; }

        [PropertyPresentation("String", "Postal address", isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 200)]
        public string PostalAddress { get; set; }

        [PropertyPresentation("String", "Postal code", isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 200)]
        public string PostalCode { get; set; }

        [PropertyPresentation("String", null, isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 200)]
        public string City { get; set; }

        [PropertyPresentation("Country", "Country", isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 200)]
        public string Country { get; set; }

        [PropertyPresentation("String", null, isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 200)]
        public string Phone { get; set; }

        [PropertyPresentation("String", null, isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 200)]
        public string Email { get; set; }

        [PropertyPresentation("String", "Registration number", isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 200)]
        public string RegistrationNumber { get; set; }

        [PropertyPresentation("NumberString", "Total price excl tax", isEditable: false, isFilterable: true, isGridColumn: true, sortOrder: 200)]
        public decimal TotalPriceExclTax { get; set; }

        [PropertyPresentation("NumberString", "Total price incl tax", isEditable: false, isFilterable: true, isGridColumn: true, sortOrder: 200)]
        public decimal TotalPriceInclTax { get; set; }

        
        [PropertyPresentation("String", "Currency", isEditable: false, isFilterable: true, isGridColumn: true, sortOrder: 200)]
        public string Currency { get; set; }
        
        [PropertyPresentation("OrderStatus", "Shipping status", isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 200)]
        public string ShippingStatus { get; set; }

        [PropertyPresentation("Textarea", null, isEditable: true, isFilterable: false, isGridColumn: false, sortOrder: 200)]
        public string Comment { get; set; }

        public int CountryId { get; set; }

        [PropertyPresentation("OrderProductList", "", isEditable: false, isFilterable: false, isGridColumn: false, languageDependant: true, sortOrder: 200)]
        public List<ProductOrderItemModel> ProductOrderItems { get; set; }

        [PropertyPresentation("OrderPromoList", "", isEditable: false, isFilterable: false, isGridColumn: false, languageDependant: true, sortOrder: 200)]
        public List<PromoOrderItemModel> PromoOrderItems { get; set; }

        [PropertyPresentation("OrderShippingItem", "", isEditable: false, isFilterable: false, isGridColumn: false, languageDependant: true, sortOrder: 200)]
        public ShippingOrderItemModel ShippingOrderItem { get; set; }

        [PropertyPresentation("OrderPaymentItem", "", isEditable: false, isFilterable: false, isGridColumn: false, languageDependant: true, sortOrder: 200)]
        public PaymentOrderItemModel PaymentOrderItem { get; set; }

        [JsonIgnore]
        [PropertyPresentation("OrderStatusHistory", "", isEditable: false, isFilterable: false, isGridColumn: false, languageDependant: true, sortOrder: 300)]
        public List<ShippingHistoryModel> ShippingHistory { get; set; }

        public override List<string> GetDefaultPropertiesInGrid()
        {
            return new List<string>() { "Identifier", "CreatedAt", "FirstName", "LastName", "ShippingStatus", "TotalPriceExclTax", "Currency" };
        }
    }
}
