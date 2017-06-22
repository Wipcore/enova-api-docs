using Wipcore.Enova.Api.Abstractions.Attributes;

namespace Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.Order
{
    public class PaymentOrderItemModel : BaseModel
    {
        [PropertyPresentation("String", "Payment name", languageDependant: true, isEditable: false, isFilterable: true, isGridColumn: false, sortOrder: 13600)]
        
        public string Name { get; set; }

        [PropertyPresentation("NumberString", "Payment id", isEditable: false, isFilterable: true, isGridColumn: false, sortOrder: 13700)]
        public string PaymentID { get; set; }

        [PropertyPresentation("String", "Payment identifier", isEditable: false, isFilterable: true, isGridColumn: false, sortOrder: 13700)]
        public string PaymentIdentifier { get; set; }

        [PropertyPresentation("NumberString", "Payment amount", isEditable: false, isFilterable: true, isGridColumn: false, sortOrder: 13800)]
        public decimal Amount { get; set; }

        [PropertyPresentation("Boolean", "Paid", isEditable: false, isFilterable: true, isGridColumn: false, sortOrder: 13800)]
        public bool Paid { get; set; }

        public string PaymentDate { get; set; }
    }
}
