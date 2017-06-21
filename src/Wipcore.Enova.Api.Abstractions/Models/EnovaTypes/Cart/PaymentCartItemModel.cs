using Wipcore.Enova.Api.Abstractions.Attributes;

namespace Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.Cart
{
    public class PaymentCartItemModel : BaseModel
    {
        [PropertyPresentation("String", "Payment name", languageDependant: true, isEditable: false, isFilterable: true, isGridColumn: true, sortOrder: 9300)]

        public string Name { get; set; }

        [PropertyPresentation("NumberString", "Payment id", isEditable: false, isFilterable: true, isGridColumn: true, sortOrder: 9400)]
        public string PaymentID { get; set; }

        [PropertyPresentation("String", "Payment identifier", isEditable: false, isFilterable: true, isGridColumn: true, sortOrder: 9400)]
        public string PaymentIdentifier { get; set; }

        [PropertyPresentation("NumberString", "Payment price excl tax", isEditable: false, isFilterable: true, isGridColumn: true, sortOrder: 9500)]
        public decimal PriceExclTax { get; set; }

        [PropertyPresentation("NumberString", "Payment price incl tax", isEditable: false, isFilterable: true, isGridColumn: true, sortOrder: 9600)]
        public decimal PriceInclTax { get; set; }

        public bool MarkForDelete { get; set; }

        public override string ToString()
        {
            return PaymentIdentifier;
        }
    }
}
