using System.Collections.Generic;
using Wipcore.Enova.Api.Abstractions.Attributes;
using Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.Attribute;

namespace Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.Product
{
    [GroupPresentation("Product info", new string[] { "Identifier", "Name", "DescriptionShort", "DescriptionLong" }, sortOrder: 100)]
    [GroupPresentation("Prices", new string[] { "Prices" }, sortOrder: 200)]
    [GroupPresentation("Attributes", new string[] { "Attributes" }, sortOrder: 300)]
    [GroupPresentation("Variants", new string[] { "VariantOwner", "Variants" }, sortOrder: 400)]
    [GroupPresentation("Images", new string[] { "ImageMaster", "Images" }, sortOrder: 450)]
    [GroupPresentation("Stock", new string[] { "IsNotStockable", "Compartments", "StockHistory" }, sortOrder: 500)]
    [IndexModel]
    public class ProductModel : BaseModel
    {
        [PropertyPresentation("String", null, languageDependant: true, isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 300)]
        public string Name { get; set; }
        
        [PropertyPresentation("Textarea", "Short description", languageDependant: true, isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 300)]
        public string DescriptionShort { get; set; }

        [PropertyPresentation("Textarea", "Long description", languageDependant: true, isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 300)]
        public string DescriptionLong { get; set; }

        [PropertyPresentation("ImageMaster", "Main image", languageDependant: false, isEditable: true, isFilterable: true, isGridColumn: false, sortOrder: 300)]
        public string ImageMaster { get; set; }

        [PropertyPresentation("ImagesExtra", "", languageDependant: true, isEditable: true, isFilterable: false, isGridColumn: false, sortOrder: 300)]
        public List<ImageModel> Images { get; set; }

        [PropertyPresentation("String", "Url", isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 300)]
        public string URL { get; set; }

        [PropertyPresentation("PricesOnProduct", "", isEditable: false, isFilterable: false, isGridColumn: false, sortOrder: 300)]
        public List<ProductPriceModel> Prices { get; set; }

        [PropertyPresentation("ProductVariantOwner", "", isEditable: false, isFilterable: false, isGridColumn: false, languageDependant: true, sortOrder: 300)]
        public VariantOwnerModel VariantOwner { get; set; }

        public int? VariantOwnerId { get; set; }
        

        [PropertyPresentation("ProductVariants", "", isEditable: true, isFilterable: false, isGridColumn: false, languageDependant: true, sortOrder: 300)]
        public List<VariantModel> Variants { get; set; }
        
        public List<int> VariantIds { get; set; }

        [PropertyPresentation("AttributesOnObject", "", isEditable: true, isFilterable: false, isGridColumn: false, languageDependant: true, sortOrder: 300)]
        public List<AttributeValueModel> Attributes { get; set; }

        [PropertyPresentation("Boolean", "Is not stockable", description: "Set true if this product should not have any stock in Enova.", isEditable: true, isFilterable: true, isGridColumn: false, languageDependant: false, sortOrder: 300)]
        public bool IsNotStockable { get; set; }

        [PropertyPresentation("Compartments", "", isEditable: true, isFilterable: false, isGridColumn: false, languageDependant: true, sortOrder: 300)]
        public List<StockInformationCompartmentModel> Compartments { get; set; }

        [PropertyPresentation("StockHistory", "", isEditable: true, isFilterable: false, isGridColumn: false, sortOrder: 300)]
        public List<StockInformationHistoryModel> StockHistory { get; set; }

        public override List<string> GetDefaultPropertiesInGrid()
        {
            return new List<string>() { "Identifier", "Name", "Enabled", "ModifiedAt", "DescriptionShort" };
        }

    }
}
