using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using IdentityModel;
using IdentityServer4.Core;
using Microsoft.AspNet.Mvc;

namespace Wipcore.Enova.Api.OAuth
{
    [Route("[controller]")]
    public class AccountController : Controller
    {
        private readonly LoginService _loginService;

        public AccountController(LoginService loginService)
        {
            _loginService = loginService;
        }

        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.Authentication.SignOutAsync(Constants.PrimaryAuthenticationType);
            return Ok("Successful logout.");
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody]LoginModel model)
        {
            if (!ModelState.IsValid)
                return HttpBadRequest("Username and password required.");

            var user = _loginService.Login(model);

            if (user == null)
                return HttpBadRequest("Invalid username or password.");

            var claims = new Claim[] {
                new Claim(JwtClaimTypes.Subject, user.Alias),
                new Claim(JwtClaimTypes.Name, user.FirstNameLastName),
                new Claim(JwtClaimTypes.IdentityProvider, "idsvr"),
                new Claim(JwtClaimTypes.AuthenticationTime, DateTime.UtcNow.ToEpochTime().ToString()),
                new Claim(JwtClaimTypes.Id, user.ID.ToString()), 
                new Claim(AuthConstants.IdentifierClaim, user.Identifier), 
                new Claim(JwtClaimTypes.Role, model.IsAdmin ? AuthConstants.AdminRole : AuthConstants.CustomerRole), 
            };
            var ci = new ClaimsIdentity(claims, "password", JwtClaimTypes.Name, JwtClaimTypes.Role);
            var cp = new ClaimsPrincipal(ci);

            await HttpContext.Authentication.SignInAsync(Constants.PrimaryAuthenticationType, cp);
            
            //if (model.SignInId != null)
            //{
            //    return new SignInResult(model.SignInId);
            //}

            //return Redirect("~/");
            return Ok("Successful login.");
        }
    }
}
