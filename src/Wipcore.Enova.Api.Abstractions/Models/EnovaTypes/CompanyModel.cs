using System.Collections.Generic;
using Wipcore.Enova.Api.Abstractions.Attributes;
using Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.Customer;
using Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.PriceList;

namespace Wipcore.Enova.Api.Abstractions.Models.EnovaTypes
{
    [GroupPresentation("Company", new string[] { "Identifier", "Name" }, sortOrder: 100)]
    [GroupPresentation("Contact", new string[] { "Phone", "Fax" }, sortOrder: 200)]
    [GroupPresentation("Address", new string[] { "Street", "PostalAddress", "PostalCode", "City", "Country", "CoAddress" }, sortOrder: 300)]
    [GroupPresentation("Group members", new string[] { "Users" }, sortOrder: 400)]
    [GroupPresentation("Pricelists", new string[] { "PriceLists" }, sortOrder: 500)]
    [IndexModel]
    public class CompanyModel : BaseModel
    {
        [PropertyPresentation("String", "Name", isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 150)]
        public string Name { get; set; }

        [PropertyPresentation("String", null, isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 200)]
        public string Street { get; set; }

        [PropertyPresentation("String", null, isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 200)]
        public string State { get; set; }

        [PropertyPresentation("String", "Postal address", isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 200, description: "Can be the same as City, but can also be the district of the address")]
        public string PostalAddress { get; set; }

        [PropertyPresentation("String", "Postal code", isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 200)]
        public string PostalCode { get; set; }

        [PropertyPresentation("String", null, isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 200)]
        public string City { get; set; }

        [PropertyPresentation("Country", "Country", isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 200)]
        public string Country { get; set; }

        [PropertyPresentation("String", null, isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 200)]
        public string Phone { get; set; }

        [PropertyPresentation("String", null, isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 200)]
        public string Fax { get; set; }

        [PropertyPresentation("String", null, isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 200)]
        public string CoAddress { get; set; }

        [PropertyPresentation("GroupMembers", null, isEditable: true, isFilterable: false, isGridColumn: false, languageDependant: true, sortOrder: 200)]
        public List<CustomerMiniModel> Users { get; set; }

        [PropertyPresentation("PriceLists", null, isEditable: true, isFilterable: false, isGridColumn: false, languageDependant: true, sortOrder: 200)]
        public List<PriceListMiniModel> PriceLists { get; set; }


        public override List<string> GetDefaultPropertiesInGrid() => new List<string>() { "Identifier", "Name", "City", "CreatedAt" };

        public override string GetResourceName() => "companies";
    }
}
