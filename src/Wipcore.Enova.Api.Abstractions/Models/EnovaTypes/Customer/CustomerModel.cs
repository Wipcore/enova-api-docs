using System.Collections.Generic;
using Wipcore.Enova.Api.Abstractions.Attributes;
using Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.Order;

namespace Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.Customer
{
    [GroupPresentation("Customer", new string[] { "Identifier", "Alias", "UserPassword", "FirstName", "LastName", "RegistrationNumber", "Note" }, sortOrder: 100)]
    [GroupPresentation("Address", new[] { "CompanyName", "CoAddress", "Street", "PostalAddress", "PostalCode", "City", "Country", "Phone", "Email" }, sortOrder: 200)]
    [GroupPresentation("Tax and Currency", new[] { "Taxrule", "Currency" }, sortOrder: 300)]
    [GroupPresentation("Customer groups", new[] { "Groups" }, sortOrder: 400)]
    [GroupPresentation("Orders", new[] { "OrderCount", "Orders" }, sortOrder: 500)]

    [IndexModel]
    public class CustomerModel : BaseModel
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

        [PropertyPresentation("String", "Registration number", isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 200, description: "E.g. organisation number, social security number")]
        public string RegistrationNumber { get; set; }

        [PropertyPresentation("String", null, isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 200)]
        public string CoAddress { get; set; }

        [PropertyPresentation("String", "Company Name", isEditable: true, isFilterable: false, isGridColumn: true, sortOrder: 200)]
        public string CompanyName { get; set; }

        [PropertyPresentation("CustomerOrders", "", isEditable: false, isFilterable: false, isGridColumn: false, sortOrder: 200)]
        public List<OrderMiniModel> Orders { get; set; }

        [PropertyPresentation("NumberString", "Order count", isEditable: false, isFilterable: true, isGridColumn: true, sortOrder: 200)]
        public int OrderCount { get; set; }

        [PropertyPresentation("TaxRules", "Tax rules", isEditable: true, isFilterable: false, isGridColumn: false, sortOrder: 200)]
        public string Taxrule { get; set; }

        [PropertyPresentation("Currency", "Currency", isEditable: true, isFilterable: false, isGridColumn: false, sortOrder: 200)]
        public string Currency { get; set; }

        [PropertyPresentation("Textarea", null, isEditable: true, isFilterable: false, isGridColumn: false, sortOrder: 200)]
        public string Note { get; set; }

        [PropertyPresentation("CustomerGroups", "", isEditable: true, isFilterable: false, isGridColumn: false, languageDependant: true, sortOrder: 200)]
        public List<CustomerGroupMiniModel> Groups { get; set; }

        public override List<string> GetDefaultPropertiesInGrid()
        {
            return new List<string>() { "Identifier", "FirstName", "LastName", "Alias", "Email" };
        }
    }
}
