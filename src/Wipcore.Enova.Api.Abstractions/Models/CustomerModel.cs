using System.Collections.Generic;
using Wipcore.Enova.Api.Abstractions.Interfaces;

namespace Wipcore.Enova.Api.Abstractions.Models
{
    public class CustomerModel : ICustomerModel
    {
        public string Identifier { get; set; }
        public string Alias { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Street { get; set; }
        public string PostalAddress { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string CountryName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string RegistrationNumber { get; set; }
        public string CoAddress { get; set; }
        public string CompanyName { get; set; }
        public string Password { get; set; }
        public IDictionary<string, object> AdditionalValues { get; set; }
    }
}
