using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wipcore.Enova.Api.Models.Interfaces
{
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
    }
}
