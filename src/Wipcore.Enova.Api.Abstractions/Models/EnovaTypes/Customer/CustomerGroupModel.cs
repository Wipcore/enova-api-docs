using System.Collections.Generic;
using Wipcore.Enova.Api.Abstractions.Attributes;
using Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.PriceList;

namespace Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.Customer
{
    [GroupPresentation("Group info", new string[] { "Identifier", "Name"}, sortOrder: 100)]
    [GroupPresentation("Group members", new string[] { "Users" }, sortOrder: 200)]
    [GroupPresentation("Pricelists", new string[] { "PriceLists" }, sortOrder: 300)]
    [IndexModel]
    public class CustomerGroupModel : BaseModel
    {
        [PropertyPresentation("String", null, languageDependant: true, isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 300)]
        public string Name { get; set; }

        [PropertyPresentation("GroupMembers", null, isEditable: true, isFilterable: false, isGridColumn: true, sortOrder: 300)]
        public List<CustomerMiniModel> Users { get; set; }

        [PropertyPresentation("PriceLists", null, isEditable: true, isFilterable: false, isGridColumn: false, sortOrder: 200)]
        public List<PriceListMiniModel> PriceLists { get; set; }
    }
}
