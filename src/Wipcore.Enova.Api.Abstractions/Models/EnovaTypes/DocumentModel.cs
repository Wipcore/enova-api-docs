using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wipcore.Enova.Api.Abstractions.Attributes;
using Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.Attribute;

namespace Wipcore.Enova.Api.Abstractions.Models.EnovaTypes
{
    [GroupPresentation("Document info", new string[] { "Identifier", "Name", "Description" }, sortOrder: 100)]
    [GroupPresentation("Settings", new string[] { "FileName", "StartAt", "EndAt" }, sortOrder: 200)]
    [GroupPresentation("Attributes", new string[] { "Attributes" }, sortOrder: 300)]
    [IndexModel]
    public class DocumentModel : BaseModel
    {
        [PropertyPresentation("String", null, languageDependant: true, isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 300)]
        public string Name { get; set; }

        [PropertyPresentation("DateTime", "Start at", isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 300)]
        public DateTime StartAt { get; set; }

        [PropertyPresentation("DateTime", "End at", isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 300)]
        public DateTime EndAt { get; set; }

        [PropertyPresentation("String", "File path", languageDependant: true, isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 300)]
        public string FileName { get; set; }

        [PropertyPresentation("Textarea", null, languageDependant: true, isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 300)]
        public string Description { get; set; }

        [PropertyPresentation("AttributesOnObject", "", isEditable: true, isFilterable: false, isGridColumn: false, languageDependant: true, sortOrder: 300)]
        public List<AttributeValueModel> Attributes { get; set; }

        public override List<string> GetDefaultPropertiesInGrid()
        {
            return new List<string>() { "Identifier", "Name", "FileName", "StartAt", "EndAt" };
        }
    }
}
