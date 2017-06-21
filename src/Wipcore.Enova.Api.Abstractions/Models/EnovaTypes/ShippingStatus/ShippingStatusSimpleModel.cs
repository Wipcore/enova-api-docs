using Wipcore.Enova.Api.Abstractions.Attributes;

namespace Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.ShippingStatus
{
    public class ShippingStatusArrivalModel
    {
        [PropertyPresentation("NumberString", "Soruce status id", isEditable: false, isFilterable: true, isGridColumn: true, sortOrder: 10005)]
        public int ID { get; set; }

        [PropertyPresentation("String", "Soruce status identifier", isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 10006)]
        public string Identifier { get; set; }

        public bool MarkForDelete { get; set; }

        [PropertyPresentation("String", "Soruce status name", languageDependant: true, isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 10007)]
        public string Name { get; set; }
    }

    public class ShippingStatusDestinationModel
    {
        [PropertyPresentation("NumberString", "Destination status id", isEditable: false, isFilterable: true, isGridColumn: true, sortOrder: 10015)]
        public int ID { get; set; }

        [PropertyPresentation("String", "Destination status identifier", isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 10016)]
        public string Identifier { get; set; }

        public bool MarkForDelete { get; set; }

        [PropertyPresentation("String", "Destination status name", languageDependant: true, isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 10017)]
        public string Name { get; set; }
    }
}
