using System.Collections.Generic;
using Wipcore.Enova.Api.Abstractions.Attributes;

namespace Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.Administrator
{
    [GroupPresentation("Administrator", new string[] { "Identifier", "Alias", "UserPassword", "FirstName", "LastName"}, sortOrder: 100)]
    [GroupPresentation("Address", new[] { "CoAddress", "Street", "PostalAddress", "PostalCode", "City", "Country", "Phone", "Email" }, sortOrder: 200)]
    [GroupPresentation("Currency", new[] {"Currency" }, sortOrder: 300)]
    [IndexModel]
    public class AdministratorModel : BaseModel
    {
        [PropertyPresentation("String", null, isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 300, description: "Login name")]
        public string Alias { get; set; }

        [PropertyPresentation("Password", "Password", isEditable: true, isFilterable: false, isGridColumn: false)]
        public string UserPassword { get; set; }

        [PropertyPresentation("String", "First name", isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 150)]
        public string FirstName { get; set; }

        [PropertyPresentation("String", "Last name", isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 200)]
        public string LastName { get; set; }

        [PropertyPresentation("String", null, isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 200)]
        public string Street { get; set; }

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
        public string Email { get; set; }

        [PropertyPresentation("String", null, isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 200)]
        public string CoAddress { get; set; }
       
        [PropertyPresentation("Currency", "Currency", isEditable: true, isFilterable: false, isGridColumn: false, sortOrder: 200)]
        public string Currency { get; set; }

        public override List<string> GetDefaultPropertiesInGrid()
        {
            return new List<string>() { "Identifier", "Alias", "FirstName", "LastName", "Email" };
        }
    }
}
