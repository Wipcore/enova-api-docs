using System.Collections.Generic;
using Wipcore.Enova.Api.Abstractions.Attributes;

namespace Wipcore.Enova.Api.Abstractions.Models.EnovaTypes
{
    [GroupPresentation("Text info", new[] { "Identifier", "Name" }, sortOrder: 200)]
    [IndexModel]
    public class SystemTextModel : BaseModel
    {
        [PropertyPresentation("String", "Text", languageDependant: true, isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 400)]
        public string Name { get; set; }

        public override List<string> GetDefaultPropertiesInGrid()
        {
            return new List<string>() { "Identifier", "Name" };
        }
    }
}
