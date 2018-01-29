using System;
using System.Collections.Generic;
using Wipcore.Enova.Api.Abstractions.Attributes;
using Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.Customer;
using Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.Product;

namespace Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.PriceList
{
    [GroupPresentation("Pricelist info", new string[] { "Identifier", "Name", "StartAt", "EndAt", "Password", "IsDefault", "Currency" }, sortOrder: 100)]
    [GroupPresentation("Product prices", new string[] { "ProductPrices" }, sortOrder: 200)]
    [GroupPresentation("Groups with access to this pricelist", new string[] { "Groups" }, sortOrder: 300)]
    [IndexModel]
    public class PriceListModel : BaseModel
    {
        public PriceListModel()
        {
            StartAt = WipConstants.DefaultStartAtDateTime;
            EndAt = WipConstants.DefaultEndAtDateTime;
            ValidFrom = WipConstants.InvalidDateTime;
            ValidTo = WipConstants.InvalidDateTime;
        }

        [PropertyPresentation("String", null, languageDependant: true, isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 300)]
        public string Name { get; set; }

        [PropertyPresentation("DateTime", "Start at", isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 300)]
        public DateTime StartAt { get; set; }

        [PropertyPresentation("DateTime", "End at", isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 300)]
        public DateTime EndAt { get; set; }

        [PropertyPresentation("DateTime", "Valid from", isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 300)]
        public DateTime ValidFrom { get; set; }

        [PropertyPresentation("DateTime", "Valid to", isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 300)]
        public DateTime ValidTo { get; set; }

        [PropertyPresentation("String", null, isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 300)]
        public string Password { get; set; }

        [PropertyPresentation("Boolean", "Is default", isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 300)]
        public bool IsDefault { get; set; }

        [PropertyPresentation("Currency", "Currency", isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 300)]
        public string Currency { get; set; }

        [PropertyPresentation("ProductPrices", "", isEditable: true, isFilterable: false, isGridColumn: true, languageDependant: true, sortOrder: 300)]
        public List<ProductMiniModel> ProductPrices { get; set; }

        [PropertyPresentation("CustomerGroups", "", isEditable: true, isFilterable: false, isGridColumn: true, languageDependant: true, sortOrder: 300)]
        public List<GroupMiniModel> Groups { get; set; }

        public override List<string> GetDefaultPropertiesInGrid()
        {
            return new List<string>() { "Identifier", "Name", "Currency", "StartAt", "EndAt" };
        }
    }
}
