using Wipcore.Enova.Api.Abstractions.Attributes;

namespace Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.Section
{
    public class ParentSectionModel
    {
        [PropertyPresentation("NumberString", "Parent section Id", isEditable: false, isFilterable: true, isGridColumn: true, sortOrder: 16000)]
        public int ID { get; set; }

        [PropertyPresentation("String", "Parent section identifier", isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 16001)]
        public string Identifier { get; set; }

        [PropertyPresentation("String", "Parent section name", languageDependant: true, isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 16002)]
        public string Name { get; set; }

        [IgnorePropertyOnIndex]
        public bool MarkForDelete { get; set; }
    }

    public class ChildrenSectionModel
    {
        [PropertyPresentation("NumberString", "Children section Id", isEditable: false, isFilterable: true, isGridColumn: true, sortOrder: 16010)]
        public int ID { get; set; }

        [PropertyPresentation("String", "Children section identifier", isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 16011)]
        public string Identifier { get; set; }

        [PropertyPresentation("String", "Children section name", languageDependant: true, isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 16012)]
        public string Name { get; set; }

        [IgnorePropertyOnIndex]
        public bool MarkForDelete { get; set; }
    }
}
