using Wipcore.Enova.Api.Abstractions.Attributes;

namespace Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.Cart
{
    public class PromoCartItemModel : BaseModel
    {
        [PropertyPresentation("String", "Promo name", languageDependant: true, isEditable: false, isFilterable: true, isGridColumn: true, sortOrder: 11300)]
       
        public string Name { get; set; }

        [PropertyPresentation("NumberString", "Promo id", isEditable: false, isFilterable: true, isGridColumn: true, sortOrder: 11400)]
        public string PromoID { get; set; }

        [PropertyPresentation("String", "Promo identifier", isEditable: false, isFilterable: true, isGridColumn: true, sortOrder: 11400)]
        public string PromoIdentifier { get; set; }

        [PropertyPresentation("NumberString", "Promo price excl tax", isEditable: false, isFilterable: true, isGridColumn: true, sortOrder: 11500)]
        public decimal PriceExclTax { get; set; }

        [PropertyPresentation("NumberString", "Promo price incl tax", isEditable: false, isFilterable: true, isGridColumn: true, sortOrder: 11600)]
        public decimal PriceInclTax { get; set; }

        public double Quantity { get; set; }

        public bool MarkForDelete { get; set; }
    }
}
