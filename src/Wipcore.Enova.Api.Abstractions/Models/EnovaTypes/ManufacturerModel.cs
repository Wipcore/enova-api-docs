using System.Collections.Generic;
using Wipcore.Enova.Api.Abstractions.Attributes;
using Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.Product;

namespace Wipcore.Enova.Api.Abstractions.Models.EnovaTypes
{
    [GroupPresentation("Manufacturer info", new string[] { "Identifier", "Name", "DescriptionLong", "RegistrationNumber", "Url" }, sortOrder: 100)]
    [GroupPresentation("Contact", new string[] { "Phone", "Mobile", "Fax", "Email" }, sortOrder: 200)]
    [GroupPresentation("Address", new string[] { "Street", "PostalAddress", "PostalCode", "City", "Country", "CoAddress" }, sortOrder: 300)]
    [GroupPresentation("Products", new string[] { "Products" }, sortOrder: 400)]
    [IndexModel]
    public class ManufacturerModel : BaseModel
    {
        [PropertyPresentation("String", null, isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 200)]
        public string Name { get; set; }

        [PropertyPresentation("String", null, isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 200)]
        public string Street { get; set; }

        [PropertyPresentation("String", "Postal address", isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 200, description: "Can be the same as City, but can also be the district of the address")]
        public string PostalAddress { get; set; }

        [PropertyPresentation("String", "Postal code", isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 200)]
        public string PostalCode { get; set; }

        [PropertyPresentation("String", null, isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 200)]
        public string City { get; set; }

        [PropertyPresentation("String", null, isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 200)]
        public string State { get; set; }

        [PropertyPresentation("Country", "Country", isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 200)]
        public string Country { get; set; }

        [PropertyPresentation("String", null, isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 200)]
        public string Phone { get; set; }

        [PropertyPresentation("String", null, isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 200)]
        public string Fax { get; set; }

        [PropertyPresentation("String", null, isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 200)]
        public string Mobile { get; set; }

        [PropertyPresentation("String", null, isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 200)]
        public string Email { get; set; }

        [PropertyPresentation("String", null, isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 200)]
        public string Url { get; set; }

        [PropertyPresentation("String", "Registration number", isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 200, description: "E.g. organisation number, social security number")]
        public string RegistrationNumber { get; set; }

        [PropertyPresentation("Textarea", "Description", isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 200)]
        public string DescriptionLong { get; set; }

        [PropertyPresentation("String", null, isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 200)]
        public string CoAddress { get; set; }

        [PropertyPresentation("ManufacturerProducts", "", isEditable: true, isFilterable: false, isGridColumn: false, languageDependant: true, sortOrder: 200)]
        public List<ProductMiniModel> Products { get; set; }

        public override List<string> GetDefaultPropertiesInGrid() => new List<string>() { "Identifier", "Name", "City", "Email", "CreatedAt" };
    }
}
