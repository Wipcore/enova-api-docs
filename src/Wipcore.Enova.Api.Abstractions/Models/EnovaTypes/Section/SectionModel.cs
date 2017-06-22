using System.Collections.Generic;
using Wipcore.Enova.Api.Abstractions.Attributes;
using Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.Product;

namespace Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.Section
{
    [GroupPresentation("Section info", new string[] { "Identifier", "Name", "DescriptionShort", "DescriptionLong" }, sortOrder: 200)]
    [GroupPresentation("Relations", new string[] { "Parent", "Children" }, sortOrder: 300)]
    [GroupPresentation("Products", new string[] { "Products" }, sortOrder: 400)]
    [IndexModel]
    public class SectionModel : BaseModel
    {
        [PropertyPresentation("String", null, languageDependant: true, isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 300)]
        public string Name { get; set; }

        [PropertyPresentation("Textarea", "Short description", languageDependant: true, isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 300)]
        public string DescriptionShort { get; set; }

        [PropertyPresentation("Textarea", "Long description", languageDependant: true, isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 300)]
        public string DescriptionLong { get; set; }

        [PropertyPresentation("SectionParent", null, isEditable: true, isFilterable: false, isGridColumn: false, languageDependant: true, sortOrder: 300)]
        public ParentSectionModel Parent { get; set; }

        [PropertyPresentation("SectionChildren", null, isEditable: true, isFilterable: false, isGridColumn: false, languageDependant: true, sortOrder: 300)]
        public List<ChildrenSectionModel> Children { get; set; }

        [PropertyPresentation("SectionProducts", null, isEditable: true, isFilterable: false, isGridColumn: false, languageDependant: true, sortOrder: 300)]
        public List<ProductMiniModel> Products { get; set; }

        public override List<string> GetDefaultPropertiesInGrid() => new List<string>() { "Identifier", "Name", "DescriptionShort", "CreatedAt", "ModifiedAt" };
    }
}
