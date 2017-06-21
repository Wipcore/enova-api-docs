using System;
using System.Collections.Generic;
using Wipcore.Enova.Api.Abstractions.Attributes;

namespace Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.Cart
{
    [GroupPresentation("Cart info", new string[] { "Identifier", "Name", "CustomerIdentifier", "TotalPriceExclTax", "TotalPriceInclTax", "Currency", "CustomerComment"}, sortOrder: 200)]
    [GroupPresentation("Products", new string[] { "ProductCartItems"}, sortOrder: 300)]
    [GroupPresentation("Promotions", new string[] { "PromoCartItems" , "PromoCode" }, sortOrder: 310)]
    [GroupPresentation("Shipping", new string[] { "ShippingCartItem", "NewShippingType" }, sortOrder: 320)]
    [GroupPresentation("Payment", new string[] { "PaymentCartItem", "NewPaymentType" }, sortOrder: 330)]
    [GroupPresentation("Valid time", new string[] { "StartAt", "EndAt" }, sortOrder: 400)]
    [IndexModel]
    public class CartModel : BaseModel
    {
        [PropertyPresentation("NumberString", "Customer Id", isEditable: false, isFilterable: true, isGridColumn: true, sortOrder: 300)]
        public int CustomerID { get; set; }

        [PropertyPresentation("CustomerIdentifier", "Customer identifier", isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 300)]
        public string CustomerIdentifier { get; set; }

        [PropertyPresentation("String", null, isEditable: true, isFilterable: true, isGridColumn: true, languageDependant: true, sortOrder: 200)]
        public string Name { get; set; }
        
        [PropertyPresentation("NumberString", "Total price excl tax", isEditable: false, isFilterable: true, isGridColumn: true, sortOrder: 200)]
        public decimal TotalPriceExclTax { get; set; }

        [PropertyPresentation("NumberString", "Total price incl tax", isEditable: false, isFilterable: true, isGridColumn: true, sortOrder: 200)]
        public decimal TotalPriceInclTax { get; set; }


        [PropertyPresentation("Currency", "Currency", isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 200)]
        public string Currency { get; set; }
       

        [PropertyPresentation("Textarea", "Comment", isEditable: true, isFilterable: false, isGridColumn: false, sortOrder: 200)]
        public string CustomerComment { get; set; }

        [PropertyPresentation("DateTime", "Start at", isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 300, description: "When this cart will become active.")]
        public DateTime StartAt { get; set; } = WipConstants.DefaultStartAtDateTime;

        [PropertyPresentation("DateTime", "End at", isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 300, description: "When this cart will stop being active.")]
        public DateTime EndAt { get; set; } = WipConstants.DefaultEndAtDateTime;

        [PropertyPresentation("CartProductList", "", isEditable: true, isFilterable: false, isGridColumn: false, languageDependant: true, sortOrder: 200)]
        public List<ProductCartItemModel> ProductCartItems { get; set; } = new List<ProductCartItemModel>();

        [PropertyPresentation("CartPromoList", "", isEditable: true, isFilterable: false, isGridColumn: false, languageDependant: true, sortOrder: 200)]
        public List<PromoCartItemModel> PromoCartItems { get; set; } = new List<PromoCartItemModel>();

        [PropertyPresentation("CartPaymentItem", "", isEditable: true, isFilterable: false, isGridColumn: true, languageDependant: true, sortOrder: 200)]
        public PaymentCartItemModel PaymentCartItem { get; set; }

        [PropertyPresentation("PaymentTypes", "Set payment type", isEditable: true, isFilterable: false, isGridColumn: true, sortOrder: 200)]
        public string NewPaymentType { get; set; }

        [PropertyPresentation("CartShippingItem", "", isEditable: false, isFilterable: false, isGridColumn: true, languageDependant: true, sortOrder: 200)]
        public ShippingCartItemModel ShippingCartItem { get; set; }

        [PropertyPresentation("ShippingTypes", "Set shipping type", isEditable: true, isFilterable: false, isGridColumn: true, sortOrder: 200)]
        public string NewShippingType { get; set; }

        [PropertyPresentation("String", "Promo code", isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 200, description: "Put a code here to unlock a password protected promotion.")]
        public string PromoCode { get; set; }

        public override List<string> GetDefaultPropertiesInGrid() => new List<string>() {"Identifier", "Name", "TotalPriceInclTax", "CustomerIdentifier" };
        
    }
}
