using System.Collections.Generic;
using Wipcore.Enova.Api.Abstractions.Attributes;

namespace Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.Attribute
{
    [GroupPresentation("Attribute type info", new string[] { "Identifier", "Name", "TypeDescription", "IsContinuous", "LanguageDependant" }, sortOrder: 200)]
    [GroupPresentation("Attribute values", new string[] { "Values" }, sortOrder: 300)]
    [IndexModel]
    public class AttributeTypeModel : BaseModel
    {
        [PropertyPresentation("String", null, languageDependant: true, isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 300)]
        public string Name { get; set; }

        [PropertyPresentation("Textarea", "Description", languageDependant: true, isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 300)]
        public string TypeDescription { get; set; }

        [PropertyPresentation("Boolean", "Is continuous", isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 300, description: "Continuous allows any value, instead of only previously specified values.")]
        public bool IsContinuous { get; set; }

        [PropertyPresentation("Boolean", "Is language dependant", isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 300, description: "True if the attribute has different values for different languages.")]
        [IgnorePropertyOnIndex]
        public bool LanguageDependant { get; set; }

        [PropertyPresentation("AttributeValues", "", isEditable: true, isFilterable: false, isGridColumn: false, sortOrder: 300)]
        public List<AttributeValueModel> Values { get; set; }


        public override List<string> GetDefaultPropertiesInGrid()
        {
            return new List<string>() { "Identifier", "Name", "TypeDescription", "IsContinuous" };
        }
    }
}
