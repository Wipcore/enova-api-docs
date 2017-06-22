using Wipcore.Enova.Api.Abstractions.Attributes;

namespace Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.Order
{
    public class ProductOrderItemModel : BaseModel
    {
        [PropertyPresentation("String", "Product name", languageDependant: true, isEditable: false, isFilterable: true, isGridColumn: false, sortOrder: 8200)]
     
        public string Name { get; set; }

        [PropertyPresentation("NumberString", "Product id", isEditable: false, isFilterable: true, isGridColumn: false, sortOrder: 13700)]
        public string ProductID { get; set; }

        [PropertyPresentation("String", "Product identifier", isEditable: false, isFilterable: true, isGridColumn: false, sortOrder: 8300)]
        public string ProductIdentifier { get; set; }

        [PropertyPresentation("NumberString", "Product price excl tax", isEditable: false, isFilterable: true, isGridColumn: false, sortOrder: 8400)]
        public decimal PriceExclTax { get; set; }

        [PropertyPresentation("NumberString", "Product price incl tax", isEditable: false, isFilterable: true, isGridColumn: false, sortOrder: 8500)]
        public decimal PriceInclTax { get; set; }

        [PropertyPresentation("NumberString", "Ordered quantity", isEditable: false, isFilterable: true, isGridColumn: false, sortOrder: 8600)]
        public double OrderedQuantity { get; set; }
    }
}
