using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using IdentityModel;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Wipcore.Enova.Api.Interfaces;
using Wipcore.Enova.Api.Models;
using System.Linq;

namespace Wipcore.Enova.Api.OAuth
{
    /// <summary>
    /// This controller is used to login customers and admins into Enova. It should put a cookie in the response, which should be sent back by the client on subsequent requests. 
    /// </summary>
    [Route("[controller]")]
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IConfigurationRoot _configuration;

        public AccountController(IAuthService authService, IConfigurationRoot configuration)
        {
            _authService = authService;
            _configuration = configuration;
        }

        /// <summary>
        /// Get information about any logged in user.
        /// </summary>
        [HttpGet("LoggedInAs")]
        public IDictionary<string, object> LoggedInAs()
        {
            return _authService.GetLoggedInAlias() == null ?
                new Dictionary<string, object>() { { "LoggedIn", false } } :
                new Dictionary<string, object>()
                {
                    { "LoggedIn", true }, { "Alias", _authService.GetLoggedInAlias() }, { "Identifier", _authService.GetLoggedInIdentifier() }, { "Name", _authService.GetLoggedInName() },
                    { "LoginTime", _authService.GetClaim(JwtClaimTypes.AuthenticationTime) },  { "Role", _authService.GetLoggedInRole() }, { "Id", _authService.GetLoggedInId() }
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
        [HttpPost("LoginAdmin")]
        public async Task<IActionResult> LoginAdmin([FromBody]LoginModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new LoginResponseModel("Alias and password required."));

            var claimsPrincipal = _authService.Login(model, admin: true);

            if (claimsPrincipal == null)
                return BadRequest(new LoginResponseModel("Invalid alias or password."));

            await HttpContext.Authentication.SignInAsync(AuthService.AuthenticationScheme, claimsPrincipal);
            
            var bearerToken = _authService.BuildToken(claimsPrincipal);

            return Ok(new LoginResponseModel("Successful login.", claimsPrincipal.FindFirst(AuthService.IdentifierClaim).Value, 
                claimsPrincipal.FindFirst(AuthService.IdClaim).Value, bearerToken));
        }

        /// <summary>
        /// Login as a customer.
        /// </summary>
        [HttpPost("LoginCustomer")]
        public async Task<IActionResult> LoginCustomer([FromBody]LoginModel model)
        {
            if (!ModelState.IsValid)
                return (BadRequest(new LoginResponseModel("Alias and password required.")));

            var claimsPrincipal = _authService.Login(model, admin: false);

            if (claimsPrincipal == null)
                return BadRequest(new LoginResponseModel("Invalid alias or password."));

            await HttpContext.Authentication.SignInAsync(AuthService.AuthenticationScheme, claimsPrincipal);

            var bearerToken = _authService.BuildToken(claimsPrincipal);

            return Ok(new LoginResponseModel("Successful login.", claimsPrincipal.FindFirst(AuthService.IdentifierClaim).Value,
                claimsPrincipal.FindFirst(AuthService.IdClaim).Value, bearerToken));
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
                return BadRequest(new LoginResponseModel("Alias and password required."));

            string errorMessage;
            var claimsPrincipal = _authService.LoginCustomerAsAdmin(model, out errorMessage);

            if (claimsPrincipal == null)
                return BadRequest(new LoginResponseModel(errorMessage));

            await HttpContext.Authentication.SignInAsync(AuthService.AuthenticationScheme, claimsPrincipal);

            var bearerToken = _authService.BuildToken(claimsPrincipal);

            return Ok(new LoginResponseModel("Successful login.", claimsPrincipal.FindFirst(AuthService.IdentifierClaim).Value,
                claimsPrincipal.FindFirst(AuthService.IdClaim).Value, bearerToken));
        }
    }
}
