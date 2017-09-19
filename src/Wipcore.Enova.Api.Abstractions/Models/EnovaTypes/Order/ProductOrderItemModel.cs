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

        [PropertyPresentation("String", "Product price excl tax", isEditable: false, isFilterable: true, isGridColumn: false, sortOrder: 8600)]
        public string PriceExclTaxString { get; set; }

        [PropertyPresentation("String", "Product price incl tax", isEditable: false, isFilterable: true, isGridColumn: false, sortOrder: 8700)]
        public string PriceInclTaxString { get; set; }

        [PropertyPresentation("String", "Total product price excl tax", isEditable: false, isFilterable: true, isGridColumn: false, sortOrder: 8800)]
        public string TotalPriceExclTaxString { get; set; }

        [PropertyPresentation("String", "Total product price incl tax", isEditable: false, isFilterable: true, isGridColumn: false, sortOrder: 8900)]
        public string TotalPriceInclTaxString { get; set; }

        [PropertyPresentation("NumberString", "Ordered quantity", isEditable: false, isFilterable: true, isGridColumn: false, sortOrder: 9000)]
        public double OrderedQuantity { get; set; }

        [PropertyPresentation("String", null, isEditable: true, isFilterable: false, isGridColumn: false, sortOrder: 9100)]
        public string Comment { get; set; }

    }
}
