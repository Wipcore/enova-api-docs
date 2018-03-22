using System.Collections.Generic;
using Wipcore.Enova.Api.Abstractions.Attributes;

namespace Wipcore.Enova.Api.Abstractions.Models.EnovaTypes
{
    [IndexModel]
    [GroupPresentation("Country", new string[] { "Identifier", "Name" }, sortOrder: 100)]
    public class CountryModel : BaseModel
    {
        [PropertyPresentation("String", null, isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 100)]
        public string Name { get; set; }

        public override List<string> GetDefaultPropertiesInGrid() => new List<string>() { "Identifier", "Name", "ModifiedAt" };

        public override string GetResourceName() => "countries";
    }
}
