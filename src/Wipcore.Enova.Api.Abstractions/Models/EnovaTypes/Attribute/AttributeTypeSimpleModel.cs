using System.Collections.Generic;
using Wipcore.Enova.Api.Abstractions.Attributes;

namespace Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.Attribute
{
    public class AttributeTypeSimpleModel
    {
        [PropertyPresentation("NumberString", "Attribute type id", isEditable: false, isFilterable: true, isGridColumn: true, sortOrder: 330)]
        public int ID { get; set; }

        [PropertyPresentation("String", "Attribute type identifier", isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 330)]
        public string Identifier { get; set; }

        [PropertyPresentation("String", "Attribute type name", languageDependant: true, isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 330)]
        public string Name { get; set; }

        [PropertyPresentation("Textarea", "Attribute description", languageDependant: true, isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 330)]
        public string TypeDescription { get; set; }

        [PropertyPresentation("Boolean", "Is language dependant", isEditable: true, isFilterable: false, isGridColumn: true, sortOrder: 300, description: "True if the attribute has different values for different languages.")]
        [IgnorePropertyOnIndex]
        public bool LanguageDependant { get; set; }

        public bool IsContinuous { get; set; }

        [PropertyPresentation("AttributeValues", "", languageDependant: true, isEditable: true, isFilterable: false, isGridColumn: false, sortOrder: 300)]
        public List<AttributeValueModel> Values { get; set; }

    }
}
