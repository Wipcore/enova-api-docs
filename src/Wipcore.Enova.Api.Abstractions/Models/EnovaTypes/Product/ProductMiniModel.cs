using System.Collections.Generic;
using Wipcore.Enova.Api.Abstractions.Attributes;

namespace Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.Product
{
    public class ProductMiniModel
    {
        [PropertyPresentation("String", "Product Id", isEditable: false, isFilterable: true, isGridColumn: false, sortOrder: 15200)]
        public int ID { get; set; }

        [PropertyPresentation("String", "Product identifier", isEditable: false, isFilterable: true, isGridColumn: false, sortOrder: 15201)]
        public string Identifier { get; set; }

        [PropertyPresentation("String", "Product name", isEditable: false, isFilterable: true, isGridColumn: false, languageDependant: true, sortOrder: 15202)]
        public string Name { get; set; }

        [PropertyPresentation("NumberString", "Product price excl tax", isEditable: true, isFilterable: true, isGridColumn: false, sortOrder: 15203)]
        public decimal PriceExclTax { get; set; }

        [PropertyPresentation("NumberString", "Product price incl tax", isEditable: false, isFilterable: true, isGridColumn: false, sortOrder: 15204)]
        public decimal PriceInclTax { get; set; }

        public List<string> Attributes { get; set; }

        [IgnorePropertyOnIndex]
        public bool MarkForDelete { get; set; }

    }

    public class VariantOwnerModel
    {
        [PropertyPresentation("String", "Owner Id", isEditable: false, isFilterable: true, isGridColumn: false, sortOrder: 15300)]
        public int ID { get; set; }

        [PropertyPresentation("String", "Owner identifier", isEditable: false, isFilterable: true, isGridColumn: false, sortOrder: 15301)]
        public string Identifier { get; set; }

        [PropertyPresentation("String", "Owner name", isEditable: false, isFilterable: true, isGridColumn: false, languageDependant: true, sortOrder: 15302)]
        public string Name { get; set; }

        [PropertyPresentation("NumberString", "Owner price excl tax", isEditable: true, isFilterable: false, isGridColumn: false, sortOrder: 15303)]
        public decimal PriceExclTax { get; set; }

        [PropertyPresentation("NumberString", "Owner price incl tax", isEditable: false, isFilterable: false, isGridColumn: false, sortOrder: 15304)]
        public decimal PriceInclTax { get; set; }

        public List<string> Attributes { get; set; }

        [IgnorePropertyOnIndex]
        public bool MarkForDelete { get; set; }
    }

    public class VariantModel
    {
        [PropertyPresentation("String", "Variant Id", isEditable: false, isFilterable: true, isGridColumn: false, sortOrder: 15400)]
        public int ID { get; set; }

        [PropertyPresentation("String", "Variant identifier", isEditable: false, isFilterable: true, isGridColumn: false, sortOrder: 15401)]
        public string Identifier { get; set; }

        [PropertyPresentation("String", "Variant name", isEditable: false, isFilterable: true, isGridColumn: false, languageDependant: true, sortOrder: 15402)]
        public string Name { get; set; }

        [PropertyPresentation("NumberString", "Variant price excl tax", isEditable: true, isFilterable: false, isGridColumn: false, sortOrder: 15403)]
        public decimal PriceExclTax { get; set; }

        [PropertyPresentation("NumberString", "Variant price incl tax", isEditable: false, isFilterable: false, isGridColumn: false, sortOrder: 15404)]
        public decimal PriceInclTax { get; set; }

        public List<string> Attributes { get; set; }

        [IgnorePropertyOnIndex]
        public bool MarkForDelete { get; set; }
    }
}
