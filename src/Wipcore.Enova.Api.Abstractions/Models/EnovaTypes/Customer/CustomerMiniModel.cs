using Wipcore.Enova.Api.Abstractions.Attributes;

namespace Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.Customer
{
    public class CustomerMiniModel : BaseModel
    {
        [PropertyPresentation("String", "Customer Id", isEditable: false, isFilterable: true, isGridColumn: true, sortOrder: 1000)]
        public new int ID { get; set; }

        [PropertyPresentation("String", "Customer identifier", isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 1001)]
        public new string Identifier { get; set; }

        [PropertyPresentation("String", "Customer alias", isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 1002)]
        public string Alias { get; set; }

        [PropertyPresentation("String", "Customer firstname", isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 1003)]
        public string FirstName { get; set; }

        [PropertyPresentation("String", "Customer lastname", isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 1004)]
        public string LastName { get; set; }

        [IgnorePropertyOnIndex]
        public bool MarkForDelete { get; set; }
    }
}
