using Wipcore.Enova.Api.Abstractions.Attributes;

namespace Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.Cart
{
    public class ShippingCartItemModel : BaseModel
    {
        [PropertyPresentation("String", "Shipping name", languageDependant: true, isEditable: false, isFilterable: true, isGridColumn: true, sortOrder: 9300)]

        public string Name { get; set; }

        [PropertyPresentation("String", "Shipping identifier", isEditable: false, isFilterable: true, isGridColumn: true, sortOrder: 9401)]
        public string ShippingIdentifier { get; set; }

        [PropertyPresentation("NumberString", "Shipping id", isEditable: false, isFilterable: true, isGridColumn: true, sortOrder: 9400)]
        public string ShippingID { get; set; }

        [PropertyPresentation("NumberString", "Shipping price excl tax", isEditable: false, isFilterable: true, isGridColumn: true, sortOrder: 9500)]
        public decimal PriceExclTax { get; set; }

        [PropertyPresentation("NumberString", "Shipping price incl tax", isEditable: false, isFilterable: true, isGridColumn: true, sortOrder: 9600)]
        public decimal PriceInclTax { get; set; }

        public bool MarkForDelete { get; set; }

        public override string ToString()
        {
            return ShippingIdentifier;
        }
    }
}
