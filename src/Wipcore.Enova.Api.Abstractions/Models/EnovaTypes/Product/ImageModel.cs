using Wipcore.Enova.Api.Abstractions.Attributes;

namespace Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.Product
{
    public class ImageModel
    {
        [PropertyPresentation("String", "Images Id", isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 600)]
        public int ID { get; set; }

        [PropertyPresentation("String", "Images identifier", isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 600)]
        public string Identifier { get; set; }

        [PropertyPresentation("String", "Images name", languageDependant: true, isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 601)]
        public string Name { get; set; }

        [PropertyPresentation("String", "Images path", languageDependant: true, isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 602)]
        public string Path { get; set; }

        [IgnorePropertyOnIndex]
        public bool MarkForDelete { get; set; }
    }
}
