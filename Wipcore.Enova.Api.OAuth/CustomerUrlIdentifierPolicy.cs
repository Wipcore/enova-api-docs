using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNet.Authorization;
using Wipcore.Enova.Api.Interfaces;

namespace Wipcore.Enova.Api.OAuth
{
    /// <summary>
    /// This policy specifices that a logged in customer can only see it's own information. User taken from identifier parameter in url.
    /// Admins can see all information.
    /// </summary>
    public class CustomerUrlIdentifierPolicy : AuthorizationHandler<CustomerUrlIdentifierPolicy>, IAuthorizationRequirement
    {
        public const string Name = "UrlIdentifier";

        protected override void Handle(AuthorizationContext context, CustomerUrlIdentifierPolicy requirement)
        {
            if (context.User.HasClaim(x => x.Type == JwtClaimTypes.Role && x.Value == AuthService.AdminRole))
            {
                context.Succeed(requirement);
                return;
            }
            var resource = (context.Resource as Microsoft.AspNet.Mvc.Filters.AuthorizationContext);
            var identifier = resource?.RouteData?.Values?.FirstOrDefault(x => x.Key.Equals("identifier", StringComparison.InvariantCultureIgnoreCase)).Value;

            var userIdentifier = context.User.FindFirst(AuthService.IdentifierClaim)?.Value;

            if(userIdentifier == identifier?.ToString())
                context.Succeed(requirement);
            else
                context.Fail();
        }
    }
}
