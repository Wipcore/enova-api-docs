using System.Collections.Generic;

namespace Wipcore.Enova.Api.Abstractions.Interfaces
{
    /// <summary>
    /// Model for customer information.
    /// </summary>
    public interface ICustomerModel
    {
        string Identifier { get; set; }
        string Alias { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
        string Street { get; set; }
        string PostalAddress { get; set; }
        string PostalCode { get; set; }
        string City { get; set; }
        string CountryName { get; set; }
        string Phone { get; set; }
        string Email { get; set; }
        string RegistrationNumber { get; set; }
        string CoAddress { get; set; }
        string CompanyName { get; set; }
        string Password { get; set; }

        IDictionary<string, object> AdditionalValues { get; set; }
    }
}
