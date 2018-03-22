using Wipcore.Enova.Api.Abstractions.Attributes;
using Wipcore.Enova.Api.Abstractions.Models.EnovaTypes;

namespace Wipcore.Enova.Api.Abstractions.Models
{
    [IndexModel]
    public class CountryModel : BaseModel
    {
        [PropertyPresentation("String", null, isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 100)]
        public string Name { get; set; }
        public override string GetResourceName() => "countries";
    }
}
