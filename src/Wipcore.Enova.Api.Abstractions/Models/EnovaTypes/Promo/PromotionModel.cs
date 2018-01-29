using System;
using System.Collections.Generic;
using Wipcore.Enova.Api.Abstractions.Attributes;
using Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.Customer;

namespace Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.Promo
{
    [GroupPresentation("Promo info", new[] { "Identifier", "Name", "StartAt", "EndAt" }, sortOrder: 200)]
    [GroupPresentation("Settings", new[] { "MaxUse", "MaxUsePerOrder", "MaxUseTotal", "Password" }, sortOrder: 300)]
    [GroupPresentation("Conditions", new[] { "Conditions" }, sortOrder: 400)]
    [GroupPresentation("Results", new[] { "Results" }, sortOrder: 500)]
    [GroupPresentation("Access", new[] { "Groups" }, sortOrder: 600)]
    [IndexModel]
    public class PromotionModel : BaseModel
    {

        [PropertyPresentation("String", "Name", languageDependant: true, isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 300)]
        public string Name { get; set; }

        [PropertyPresentation("NumberString", "Max total uses", isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 300)]
        public int MaxUseTotal { get; set; }

        [PropertyPresentation("NumberString", "Max uses per order", isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 300)]
        public int MaxUsePerOrder { get; set; }

        [PropertyPresentation("NumberString", "Max uses per customer", isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 300)]
        public int MaxUse { get; set; }
        
        [PropertyPresentation("String", "Promo code", isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 300)]
        public string Password { get; set; }

        [PropertyPresentation("DateTime", "Start at", isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 300)]
        public DateTime StartAt { get; set; } = WipConstants.DefaultStartAtDateTime;

        [PropertyPresentation("DateTime", "End at", isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 300)]
        public DateTime EndAt { get; set; } = WipConstants.DefaultEndAtDateTime;

        [PropertyPresentation("PromoConditions", "", isEditable: true, isFilterable: false, isGridColumn: false, sortOrder: 300)]
        public PromoConditionsModel Conditions { get; set; } = new PromoConditionsModel();

        [PropertyPresentation("PromoResults", "", isEditable: true, isFilterable: false, isGridColumn: false, sortOrder: 300)]
        public PromoResultModel Results { get; set; } = new PromoResultModel();

        [PropertyPresentation("CustomerGroups", "", isEditable: true, isFilterable: false, isGridColumn: false, languageDependant: true, sortOrder: 300)]
        public List<GroupMiniModel> Groups { get; set; } = new List<GroupMiniModel>();

        public override List<string> GetDefaultPropertiesInGrid() => new List<string>() { "Identifier", "Name", "StartAt", "EndAt", "Password" };
        
    }
}
