using System;
using System.Collections.Generic;
using System.Security.Claims;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Abstractions.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace Wipcore.Enova.Api.Abstractions.Internal
{
    public interface IAuthService
    {
        /// <summary>
        /// Login user by information in model. Returns a claimprincipal with the claims of the logged in user.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="admin">True to log in as an administrator.</param>
        /// <returns></returns>
        ClaimsPrincipal Login(ILoginModel model, bool admin);

        /// <summary>
        /// Log in a customer with the credentials of an admin, ie inpersonation.
        /// </summary>
        ClaimsPrincipal LoginCustomerAsAdmin(ILoginCustomerWithAdminCredentialsModel model, out string errorMessage);

        string BuildToken(ClaimsPrincipal claimsPrincipal);

        /// <summary>
        /// Get the alias of any logged in user. Returns null if no-one logged in.
        /// </summary>
        /// <returns></returns>
        string GetLoggedInAlias();

        /// <summary>
        /// Get the name of any logged in user. Returns null if no-one logged in.
        /// </summary>
        /// <returns></returns>
        string GetLoggedInName();

        /// <summary>
        /// Get the identifier of any logged in user. Returns null if no-one logged in.
        /// </summary>
        /// <returns></returns>
        string GetLoggedInIdentifier();

        /// <summary>
        /// Get the default currency of the logged in user.
        /// </summary>

        string GetLoggedInDefaultCurrency();

        /// <summary>
        /// Get the default language of the logged in user.
        /// </summary>
        string GetLoggedInDefaultLanguage();

        /// <summary>
        /// Get the id of an logged in user. Returns null if no-one logged in.
        /// </summary>
        int? GetLoggedInId();

        /// <summary>
        /// Get the role of any logged in user. Returns null if no-one logged in.
        /// </summary>
        /// <returns></returns>
        string GetLoggedInRole();

        /// <summary>
        /// Get the time when the user was logged in.
        /// </summary>
        DateTime GetLoggedInAt();

        /// <summary>
        /// Returns true if the logged in user is an admin.
        /// </summary>
        /// <returns></returns>
        bool IsLoggedInAsAdmin();

        /// <summary>
        /// Returns true if the logged in user is a customer.
        /// </summary>
        /// <returns></returns>
        bool IsLoggedInAsCustomer();

        /// <summary>
        /// Get a claim on the logged in user by name.
        /// </summary>
        string GetClaim(string claimName);
        
        /// <summary>
        /// Returns a user friendly logging message for the logged in user.. 
        /// </summary>
        /// <returns></returns>
        string LogUser();

        /// <summary>
        /// Returns true if the current logged in user is authorized to access, by comparing with the given owner.
        /// </summary>
        /// <param name="enovaObjectOwnedByIdentifier"></param>
        /// <returns></returns>
        bool AuthorizeAccess(string enovaObjectOwnedByIdentifier);

        /// <summary>
        /// Returns true if the logged in user is an administrator, or if the object has ownerproperty that is the same as the loggedinuser by identifier OR id. 
        /// </summary>
        bool AuthorizeAccess<T>(Context context, string identifier, int id, Func<T, int?> getOwnerFunc) where T : BaseObject;

        /// <summary>
        /// Returns true if the current logged in user is authorized to update, by comparing with the given owner.
        /// </summary>
        /// <param name="enovaObjectOwnedByIdentifier">The current owner of the object.</param>
        /// <param name="specifiedOwner">The owner specified to own the object after update.</param>
        bool AuthorizeUpdate(string enovaObjectOwnedByIdentifier, string specifiedOwner);

        /// <summary>
        /// Returns true if the logged in user is an administrator, or if the object has ownerproperty that is the same as the loggedinuser.
        /// </summary>
        bool AuthorizeAccess<T>(Context context, Dictionary<string, object> values, Func<T, int?> getOwnerFunc) where T : BaseObject;

        /// <summary>
        /// Validator for when tokens expire.
        /// </summary>
        bool ExpireValidator(DateTime? notBefore, DateTime? expires, SecurityToken securityToken, TokenValidationParameters validationParameters);
    }
}