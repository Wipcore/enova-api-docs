using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;

namespace Wipcore.Enova.Api.OAuth
{
    /// <summary>
    /// This policy specifices that a logged in customer can only see it's own information. User taken from identifier parameter in body.
    /// Admins can see all information.
    /// </summary>
    public class CustomerBodyIdentifierPolicy : AuthorizationHandler<CustomerBodyIdentifierPolicy>, IAuthorizationRequirement
    {
        public const string Name = "BodyIdentifier";

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CustomerBodyIdentifierPolicy requirement)
        {
            if (context.User.HasClaim(x => x.Type == JwtClaimTypes.Role && x.Value == AuthService.AdminRole))
            {
                context.Succeed(requirement);
                return Task.FromResult(0);
            }

            var userIdentifier = context.User.FindFirst(AuthService.IdentifierClaim)?.Value;

            if (userIdentifier == null)
            {
                context.Fail(); //must be specified if inputing body as customer, otherwise a new object is created
                return Task.FromResult(0);
            }

            var resource = (context.Resource as Microsoft.AspNetCore.Mvc.Filters.AuthorizationFilterContext);
            
            var bodyStream = resource?.HttpContext?.Request?.Body;
            if (bodyStream == null)
            {
                context.Fail();
                return Task.FromResult(0);
            }
            
            if (bodyStream.CanSeek)
                bodyStream.Position = 0;
            var body = new StreamReader(bodyStream).ReadToEnd();
            var dictionary = JsonConvert.DeserializeObject<IDictionary<string, object>>(body);
            var identifier = dictionary?.FirstOrDefault(x => x.Key.Equals("identifier", StringComparison.InvariantCultureIgnoreCase)).Value;

            if (userIdentifier == identifier?.ToString())
                context.Succeed(requirement);
            else
                context.Fail();

            return Task.FromResult(0);
        }
    }
}
