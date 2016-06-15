using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using IdentityModel;
using IdentityServer4.Core;
using Microsoft.AspNet.Http.Authentication;
using Microsoft.AspNet.Mvc;
using Wipcore.Enova.Api.Interfaces;
using Wipcore.Enova.Api.Models;

namespace Wipcore.Enova.Api.OAuth
{
    /// <summary>
    /// This controller is used to login customers and admins into Enova. It should put a cookie in the response, which should be sent back by the client on subsequent requests. 
    /// </summary>
    [Route("[controller]")]
    public class AccountController : Controller
    {
        private readonly IAuthService _loginService;

        public AccountController(IAuthService loginService)
        {
            _loginService = loginService;
        }

        /// <summary>
        /// Get information about any logged in user.
        /// </summary>
        [HttpGet("LoggedInAs")]
        public IDictionary<string, object> LoggedInAs()
        {
            return _loginService.GetLoggedInAlias() == null ?
                new Dictionary<string, object>() { { "LoggedIn", false } } :
                new Dictionary<string, object>()
                {
                    { "LoggedIn", true }, { "Alias", _loginService.GetLoggedInAlias() }, { "Identifier", _loginService.GetLoggedInIdentifier() }, { "Name", _loginService.GetLoggedInName() },
                    { "LoginTime", _loginService.GetClaim(JwtClaimTypes.AuthenticationTime) },  { "Role", _loginService.GetLoggedInRole() }
                };
        }

        /// <summary>
        /// Logout from Enova.
        /// </summary>
        /// <returns></returns>
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.Authentication.SignOutAsync(AuthService.AuthenticationScheme);
            return Ok("Successful logout.");
        }

        /// <summary>
        /// Login as an administrator.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("LoginAdmin")]
        public async Task<IActionResult> LoginAdmin([FromBody]LoginModel model)
        {
            if (!ModelState.IsValid)
                return HttpBadRequest(new LoginResponseModel("Alias and password required."));

            var claimsPrincipal = _loginService.Login(model, admin: true);

            if (claimsPrincipal == null)
                return HttpBadRequest(new LoginResponseModel("Invalid alias or password."));

            await HttpContext.Authentication.SignInAsync(AuthService.AuthenticationScheme, claimsPrincipal);

            return Ok(new LoginResponseModel("Successful login.", claimsPrincipal.FindFirst(AuthService.IdentifierClaim).Value));
        }

        /// <summary>
        /// Login as a customer.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("LoginCustomer")]
        public async Task<IActionResult> LoginCustomer([FromBody]LoginModel model)
        {
            if (!ModelState.IsValid)
                return (HttpBadRequest(new LoginResponseModel("Alias and password required.")));

            var claimsPrincipal = _loginService.Login(model, admin: false);

            if (claimsPrincipal == null)
                return HttpBadRequest(new LoginResponseModel("Invalid alias or password."));

            await HttpContext.Authentication.SignInAsync(AuthService.AuthenticationScheme, claimsPrincipal);

            return Ok(new LoginResponseModel("Successful login.", claimsPrincipal.FindFirst(AuthService.IdentifierClaim).Value));
        }

        /// <summary>
        /// Login as a customer by providing a customer identifier, and the alias and password of an administrator.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("LoginCustomerWithAdminCredentials")]
        public async Task<IActionResult> LoginCustomerWithAdminCredentials([FromBody]LoginCustomerWithAdminCredentialsModel model)
        {
            if (!ModelState.IsValid)
                return HttpBadRequest(new LoginResponseModel("Alias and password required."));

            string errorMessage;
            var claimsPrincipal = _loginService.LoginCustomerAsAdmin(model, out errorMessage);

            if (claimsPrincipal == null)
                return HttpBadRequest(new LoginResponseModel(errorMessage));

            await HttpContext.Authentication.SignInAsync(AuthService.AuthenticationScheme, claimsPrincipal);

            return Ok(new LoginResponseModel("Successful login.", claimsPrincipal.FindFirst(AuthService.IdentifierClaim).Value));
        }
    }
}
