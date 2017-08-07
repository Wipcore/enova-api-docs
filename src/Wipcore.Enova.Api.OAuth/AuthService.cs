using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using IdentityModel;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Wipcore.Core;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Core;
using Wipcore.Enova.Generics;
using System.Linq;
using Wipcore.Enova.Api.Abstractions.Interfaces;
using Wipcore.Enova.Api.Abstractions.Internal;

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
        public const string IdClaim = "id";
        public const string HashClaim = "hash";
        public const string SubjectClaim = "subject";
        public const string NameClaim = "name";
        public const string RoleClaim = "role";
        public const string DefaultCurrencyClaim = "currency";
        public const string DefaultLanguageClaim = "language";

        public const string AuthenticationScheme = "EnovaApi";
        public const string DefaultSignKey = "317aff46-2ed0-4e82-9f43-47d94663887b";

        private readonly IHttpContextAccessor _httpAccessor;
        private readonly IConfigurationRoot _configuration;
        private readonly ILogger _log;
        private readonly SigningCredentials _signingCredentials;

        public AuthService(ILoggerFactory loggerFactory, IHttpContextAccessor httpAccessor, IConfigurationRoot configuration)
        {
            _httpAccessor = httpAccessor;
            _configuration = configuration;
            _log = loggerFactory.CreateLogger(GetType().Name);
            var secretKey = configuration.GetValue<string>("Auth:IssuerSigningKey", AuthService.DefaultSignKey);
            _signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)), SecurityAlgorithms.HmacSha256);
        }

        public bool ExpireValidator(DateTime? notBefore, DateTime? expires, SecurityToken securityToken, TokenValidationParameters validationParameters)
        {
            if (notBefore.HasValue && notBefore.Value > DateTime.Now)
                return false; //if not valid yet, then disallow

            if (expires.HasValue)
            {
                var nextNightFromExpire = expires.Value.AddDays(1).Date.AddHours(4);//expire next night at 4
                if (DateTime.Now > nextNightFromExpire)
                    return false;
            }

            return true;
        }


        public ClaimsPrincipal Login(ILoginModel model, bool admin)
        {
            try
            {
                var context = EnovaSystemFacade.Current.Connection.CreateContext();
                var user = admin ? (User)context.Login(model.Alias, model.Password)
                    : context.CustomerLogin(model.Alias, model.Password);

                //contextModel = new ContextModel() {Currency = user.Currency?.Identifier, Language = user.Language?.Identifier};
                var claimsPrincipal = BuildClaimsPrincipal(user, admin, model.Password);
                return claimsPrincipal;
            }
            catch (Exception e)
            {
                _log.LogError($"Error at AuthService.Login using model: {model?.ToString() ?? "null"}. Admin: {admin}. Error: {e}");
                return null;
            }
        }

        public ClaimsPrincipal LoginCustomerAsAdmin(ILoginCustomerWithAdminCredentialsModel model, out string errorMessage)
        {
            errorMessage = String.Empty;
            var context = EnovaSystemFacade.Current.Connection.CreateContext();
            try
            {
                context.Login(model.Alias, model.Password);
                var customer = context.FindObject<EnovaCustomer>(model.CustomerIdentifier);
                if (customer == null)
                {
                    errorMessage = "Customer could not be found!";
                    return null;
                }

                var user = context.CustomerLogin(customer.ID);

                var claimsPrincipal = BuildClaimsPrincipal(user, false);

                return claimsPrincipal;
            }
            catch (Exception e)
            {
                errorMessage = "Invalid username or password.";
                _log.LogError($"Error at AuthService.LoginCustomerAsAdmin using model: {model?.ToString() ?? "null"}. Error: {e}");
                return null;
            }
            finally
            {
                if (context.IsLoggedIn())
                    context.Logout();
            }
        }

        public string BuildToken(ClaimsPrincipal claimsPrincipal)
        {
            var jwt = new JwtSecurityToken(
                     issuer: AuthService.AuthenticationScheme, // Needs to be same as when checking authorization - no good error message when missaligned.
                     audience: _configuration.GetValue<string>("Auth:ValidAudience", "http://localhost:5000/"),
                     claims: claimsPrincipal.Claims.ToList(),
                     notBefore: DateTime.UtcNow,
                     expires: DateTime.UtcNow.AddMinutes(_configuration.GetValue<int>("Auth:ExpireTimeMinutes", 60)),
                     signingCredentials: _signingCredentials
                 );

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            return encodedJwt;
        }
        
        /// <summary>
        /// Save claims (attributes) on the logged in user.
        /// </summary>
        private ClaimsPrincipal BuildClaimsPrincipal(User user, bool admin, string password = null)
        {
            var claims = new List<Claim>
            {
                new Claim(SubjectClaim, user.Alias),
                new Claim(NameClaim, user.FirstNameLastName),
                new Claim(JwtClaimTypes.AuthenticationTime, DateTime.Now.ToString()),
                new Claim(IdentifierClaim, user.Identifier),
                new Claim(IdClaim, user.ID.ToString()),
                new Claim(RoleClaim, admin ? AdminRole : CustomerRole),
                new Claim(DefaultCurrencyClaim, user.Currency?.Identifier ?? ""),
                new Claim(DefaultLanguageClaim, user.Language?.Identifier ?? "")
            };

            if (admin)
                claims.Add(new Claim(HashClaim, WipUtils.CreateHashString(password)));

            var claimsIdentity = new ClaimsIdentity(claims, "password", NameClaim, RoleClaim);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            return claimsPrincipal;
        }

        public string GetLoggedInAlias() => GetClaim(SubjectClaim);

        public string GetLoggedInName() => GetClaim(NameClaim);

        public string GetLoggedInIdentifier() => GetClaim(IdentifierClaim);

        public int? GetLoggedInId() => Convert.ToInt32(GetClaim(IdClaim));

        public string GetLoggedInDefaultCurrency() => GetClaim(DefaultCurrencyClaim);

        public string GetLoggedInDefaultLanguage() => GetClaim(DefaultLanguageClaim);

        public string GetLoggedInRole()
        {
            var role = GetClaim(RoleClaim);
            if(role != null)
                return role;
            var user = _httpAccessor.HttpContext.User;
            return user.Claims.FirstOrDefault(x => x.Type.EndsWith("/" + RoleClaim))?.Value;//escaping the schema
        }

        public bool IsLoggedInAsAdmin() => GetLoggedInRole() == AdminRole;

        public bool IsLoggedInAsCustomer() => GetLoggedInRole() == CustomerRole;

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
            if (IsLoggedInAsCustomer())
                return "Customer with identifier: " + GetLoggedInIdentifier();

            return "Anonymous user";
        }

        /// <summary>
        /// Returns true if the logged in user is an administrator, or if enovaObjectOwnedByIdentifier is the same as the logged in user. 
        /// </summary>
        public bool AuthorizeAccess(string enovaObjectOwnedByIdentifier)
        {
            if (IsLoggedInAsAdmin())
                return true;

            return enovaObjectOwnedByIdentifier == null || String.Equals(enovaObjectOwnedByIdentifier, GetLoggedInIdentifier(), StringComparison.CurrentCultureIgnoreCase);
        }

        /// <summary>
        /// Returns true if the logged in user is an administrator, or if the object has ownerproperty that is the same as the loggedinuser.
        /// </summary>
        public bool AuthorizeAccess<T>(Context context, Dictionary<string, object> values, Func<T, int?> getOwnerFunc) where T : BaseObject
        {
            if (IsLoggedInAsAdmin())
                return true;

            var id = GetValueInsensitive<int>(values, "id");
            var identifier = GetValueInsensitive<string>(values, "identifier") ?? "";

            return AuthorizeAccess(context, identifier, id, getOwnerFunc);
        }

        /// <summary>
        /// Returns true if the logged in user is an administrator, or if the object has ownerproperty that is the same as the loggedinuser by identifier OR id. 
        /// </summary>
        public bool AuthorizeAccess<T>(Context context, string identifier, int id, Func<T, int?> getOwnerFunc) where T : BaseObject
        {
            if (IsLoggedInAsAdmin())
                return true;
            
            var item = id != 0 ? context.FindObject<T>(id) : context.FindObject<T>(identifier);

            if (item == null)
                return true;

            var owner = getOwnerFunc.Invoke(item);
            return owner == null || owner == GetLoggedInId();
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

        private static T GetValueInsensitive<T>(IDictionary<string, object> dictionary, string key)
        {
            var entry = dictionary.FirstOrDefault(x => String.Equals(key, x.Key, StringComparison.InvariantCultureIgnoreCase));
            if (entry.Value == null)
                return default(T);
            return (T)Convert.ChangeType(entry.Value, typeof(T));
        }
    }
}
