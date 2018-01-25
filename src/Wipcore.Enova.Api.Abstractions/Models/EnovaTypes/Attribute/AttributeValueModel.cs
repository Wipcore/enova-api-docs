using System;
using Wipcore.Enova.Api.Abstractions.Attributes;

namespace Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.Attribute
{
    public class AttributeValueModel
    {
        [PropertyPresentation("String", "Attribute Id", isEditable: false, isFilterable: true, isGridColumn: true, sortOrder: 350)]
        public int ID { get; set; }

        [PropertyPresentation("String", "Attribute identifier", isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 350)]
        public string Identifier { get; set; }

        [PropertyPresentation("DateTime", "Modified at", isEditable: false, isFilterable: false, isGridColumn: true, sortOrder: 300)]
        public DateTime ModifiedAt { get; set; }

        [PropertyPresentation("DateTime", "Created at", isEditable: false, isFilterable: false, isGridColumn: true, sortOrder: 300)]
        public DateTime CreatedAt { get; set; }

        [PropertyPresentation("Boolean", "Enabled", isEditable: true, isFilterable: false, isGridColumn: true, sortOrder: 100)]
        public bool Enabled { get; set; } = true;

        [PropertyPresentation("String", controlLabel: "Attribute value", languageDependant: true, isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 351)]
        public string Value { get; set; }

        [PropertyPresentation("String", controlLabel: "Attribute value description", languageDependant: true, isEditable: true, isFilterable: false, isGridColumn: true, sortOrder: 352)]
        public string ValueDescription { get; set; }

        public string ValueContext { get; set; }

        public int SortOrder { get; set; }

        [PropertyPresentation("String", controlLabel: "Attribute type", languageDependant: true, isEditable: true, isFilterable: false, isGridColumn: true, sortOrder: 352)]
        public AttributeTypeSimpleModel AttributeType { get; set; }

        public int ObjectCount { get; set; }

        [IgnorePropertyOnIndex]
        public bool MarkForDelete { get; set; }

        public virtual string GetResourceName() => this.GetType().Name.Replace("Model", "s").ToLower();
    }
}
