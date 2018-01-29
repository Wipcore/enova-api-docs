using Wipcore.Enova.Api.Abstractions.Attributes;

namespace Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.Customer
{
    public class GroupMiniModel
    {
        [PropertyPresentation("NumberString", "Group Id", isEditable: false, isFilterable: true, isGridColumn: true, sortOrder: 5000)]
        public int ID { get; set; }

        [PropertyPresentation("String", "Group identifier", isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 5001)]
        public string Identifier { get; set; }

        [PropertyPresentation("Boolean", "Enabled", isEditable: true, isFilterable: false, isGridColumn: true, sortOrder: 5003)]
        public bool Enabled { get; set; } = true;

        [PropertyPresentation("String", "Group name", languageDependant: true, isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 5002)]
        public string Name { get; set; }

        public string Type { get; set; }

        [IgnorePropertyOnIndex]
        public bool MarkForDelete { get; set; }
    }
}
