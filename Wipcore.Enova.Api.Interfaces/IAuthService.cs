using System.Security.Claims;
using Wipcore.Enova.Api.Models;
using Wipcore.Enova.Api.Models.Interfaces;

namespace Wipcore.Enova.Api.Interfaces
{
    public interface IAuthService
    {
        ClaimsPrincipal Login(ILoginModel model, bool admin);
        ClaimsPrincipal LoginCustomerAsAdmin(ILoginCustomerWithAdminCredentialsModel model, out string errorMessage);
        string GetLoggedInAlias();
        string GetLoggedInName();
        string GetLoggedInIdentifier();
        string GetLoggedInRole();
        bool IsLoggedInAsAdmin();
        bool IsLoggedInAsCustomer();
        string GetClaim(string claimName);
        string GetPasswordHash();
        string LogUser();
        bool AuthorizeAccess(string enovaObjectOwnedByIdentifier);
        bool AuthorizeUpdate(string enovaObjectOwnedByIdentifier, string specifiedOwner);
    }
}