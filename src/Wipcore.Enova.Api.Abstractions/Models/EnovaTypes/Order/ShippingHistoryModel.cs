using Wipcore.Enova.Api.Abstractions.Attributes;

namespace Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.Order
{
    public class ShippingHistoryModel : BaseModel
    {
        [PropertyPresentation("String", "From status", languageDependant: true, isEditable: false, isFilterable: false, isGridColumn: false, sortOrder: 300)]
        public string FromStatusName { get; set; }

        [PropertyPresentation("String", "To status", languageDependant: true, isEditable: false, isFilterable: false, isGridColumn: false, sortOrder: 300)]
        public string ToStatusName { get; set; }

        [PropertyPresentation("String", null, isEditable: false, isFilterable: false, isGridColumn: false, sortOrder: 300)]
        public string FromStatusIdentifier { get; set; }

        [PropertyPresentation("String", null, isEditable: false, isFilterable: false, isGridColumn: false, sortOrder: 300)]
        public string ToStatusIdentifier { get; set; }
    }
}
