using System.Collections.Generic;
using Wipcore.Enova.Api.Abstractions.Attributes;

namespace Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.ShippingStatus
{
    [GroupPresentation("Shipping status info", new string[] { "Identifier", "Name", "InactiveOrder", "IsScrapStatus"}, sortOrder: 100)]
    [GroupPresentation("Source connections", new string[] { "ArrivalStatuses" }, sortOrder: 200)]
    [GroupPresentation("Destination connections", new string[] { "DestinationStatuses" }, sortOrder: 300)]
    [IndexModel]
    public class ShippingStatusModel : BaseModel
    {
        [PropertyPresentation("String", null, languageDependant: true, isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 300)]
        public string Name { get; set; }

        [PropertyPresentation("Boolean", "Inactive order", isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 300)]
        public bool InactiveOrder { get; set; }

        [PropertyPresentation("Boolean", "Is scrap status", isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 300)]
        public bool IsScrapStatus { get; set; }

        [PropertyPresentation("ArrivalStatuses", "Statuses that leads to this status", isEditable: true, isFilterable: false, isGridColumn: false, languageDependant: true, sortOrder: 300)]
        public List<ShippingStatusArrivalModel> ArrivalStatuses { get; set; }

        [PropertyPresentation("DestinationStatuses", "Destination statuses", isEditable: true, isFilterable: false, isGridColumn: false, languageDependant: true, sortOrder: 300)]
        public List<ShippingStatusDestinationModel> DestinationStatuses { get; set; }

        public override List<string> GetDefaultPropertiesInGrid() => new List<string>() { "Identifier", "Name", "InactiveOrder", "IsScrapStatus" };

        public override string GetResourceName() => "shippingstatuses";
    }
}
