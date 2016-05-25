using System;
using System.Collections.Generic;
using System.Security.Claims;
using Wipcore.Core.SessionObjects;
using IdentityModel;
using Microsoft.AspNet.Http;
using Microsoft.Extensions.Logging;
using Wipcore.Core;
using Wipcore.Enova.Api.Interfaces;
using Wipcore.Enova.Api.Models;
using Wipcore.Enova.Api.Models.Interfaces;
using Wipcore.Enova.Core;

namespace Wipcore.Enova.Api.OAuth
{
    /// <summary>
    /// This services handles logging in and the setting and getting of claims.
    /// </summary>
    public class AuthService : IAuthService
    {
        public const string AdminRole = "admin";
        public const string CustomerRole = "customer";
        public const string IdentifierClaim = "identifier";
        public const string HashClaim = "hash";
        public const string AuthenticationScheme = "EnovaApi";

        private readonly IHttpContextAccessor _httpAccessor;
        private readonly ILogger _log;

        public AuthService(ILoggerFactory loggerFactory, IHttpContextAccessor httpAccessor)
        {
            _httpAccessor = httpAccessor;
            _log = loggerFactory.CreateLogger(GetType().Name);
        }

        public ClaimsPrincipal Login(ILoginModel model, bool admin)
        {
            try
            {
                var context = EnovaSystemFacade.Current.Connection.CreateContext();
                var user = admin ? (User)context.Login(model.Username, model.Password)
                    : context.CustomerLogin(model.Username, model.Password);

                var claimsPrincipal = BuildClaimsPrincipal(user, admin, model.Password);
                return claimsPrincipal;
            }
            catch (Exception e)
            {
                _log.LogError($"Error at AuthService.Login using model: {model?.ToString() ?? "null"}. Admin: {admin}. Error: {e}");
                return null;
            }
        }

        public ClaimsPrincipal LoginCustomerAsAdmin(ILoginCustomerWithAdminCredentialsModel model)
        {
            var context = EnovaSystemFacade.Current.Connection.CreateContext();
            try
            {
                context.Login(model.Username, model.Password);
                var customer = EnovaCustomer.Find(context, model.CustomerIdentifier);
                var user = context.CustomerLogin(customer.ID);

                var claimsPrincipal = BuildClaimsPrincipal(user, false);
                return claimsPrincipal;
            }
            catch (Exception e)
            {
                _log.LogError($"Error at AuthService.LoginCustomerAsAdmin using model: {model?.ToString() ?? "null"}. Error: {e}");
                return null;
            }
            finally
            {
                if (context.IsLoggedIn())
                    context.Logout();
            }
        }

        /// <summary>
        /// Save claims (attributes) on the logged in user.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="admin"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private ClaimsPrincipal BuildClaimsPrincipal(User user, bool admin, string password = null)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtClaimTypes.Subject, user.Alias),
                new Claim(JwtClaimTypes.Name, user.FirstNameLastName),
                new Claim(JwtClaimTypes.AuthenticationTime, DateTime.UtcNow.ToEpochTime().ToString()),
                new Claim(IdentifierClaim, user.Identifier),
                new Claim(JwtClaimTypes.Role, admin ? AdminRole : CustomerRole)
            };

            if (admin)
                claims.Add(new Claim(HashClaim, WipUtils.CreateHashString(password)));

            var claimsIdentity = new ClaimsIdentity(claims, "password", JwtClaimTypes.Name, JwtClaimTypes.Role);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            return claimsPrincipal;
        }

        public string GetLoggedInAlias() => GetClaim(JwtClaimTypes.Subject);

        public string GetLoggedInName() => GetClaim(JwtClaimTypes.Name);

        public string GetLoggedInIdentifier() => GetClaim(IdentifierClaim);

        public string GetLoggedInRole() => GetClaim(JwtClaimTypes.Role);

        public bool IsLoggedInAsAdmin() => GetClaim(JwtClaimTypes.Role) == AdminRole;

        public bool IsLoggedInAsCustomer() => GetClaim(JwtClaimTypes.Role) == CustomerRole;

        public string GetPasswordHash() => GetClaim(HashClaim);

        public string GetClaim(string claimName)
        {
            var user = _httpAccessor.HttpContext.User;
            return user.FindFirst(claimName)?.Value;
        }

        public string LogUser()
        {
            if (IsLoggedInAsAdmin())
                return "Admin with identifier: " + GetLoggedInIdentifier();
            if(IsLoggedInAsCustomer())
                return "Customer with identifier: " + GetLoggedInIdentifier();

            return "Anonymous user";
        }

        /// <summary>
        /// Returns true if the logged in user is an administrator, or if enovaObjectOwnedByIdentifier is the same as the logged in user.
        /// 
        /// </summary>
        /// <param name="enovaObjectOwnedByIdentifier">The user who owns the object to read, for example a customer on an order.</param>
        /// <returns></returns>
        public bool AuthorizeAccess(string enovaObjectOwnedByIdentifier)
        {
            if (IsLoggedInAsAdmin())
                return true;

            return enovaObjectOwnedByIdentifier == null || String.Equals(enovaObjectOwnedByIdentifier, GetLoggedInIdentifier(), StringComparison.CurrentCultureIgnoreCase);
        }

        /// <summary>
        /// Returns true if the logged in user is an adminstrator, 
        /// or if the logged in user is the same as the user owning the object, and the same as the user being set to own the object.
        /// </summary>
        /// <param name="enovaObjectOwnedByIdentifier">The user who owns the object to update, for example a customer on an order.</param>
        /// <param name="specifiedOwner">The user who is being set as owner for the object, for example a customer on a new order.</param>
        /// <returns></returns>
        public bool AuthorizeUpdate(string enovaObjectOwnedByIdentifier, string specifiedOwner)
        {
            if (specifiedOwner == String.Empty)
                specifiedOwner = null;

            if (IsLoggedInAsAdmin())
                return true;

            if (enovaObjectOwnedByIdentifier != null && !String.Equals(enovaObjectOwnedByIdentifier, GetLoggedInIdentifier(), StringComparison.CurrentCultureIgnoreCase))
                return false;//if owned by someone else, deny

            if (specifiedOwner != null && !String.Equals(specifiedOwner, GetLoggedInIdentifier(), StringComparison.CurrentCultureIgnoreCase))//if set to be owned by someone else, deny
                return false;

            return true;
        }
    }
}
