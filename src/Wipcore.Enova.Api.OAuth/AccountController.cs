using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.IdentityModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Abstractions;
using Wipcore.Enova.Api.Abstractions.Interfaces;
using Wipcore.Enova.Api.Abstractions.Internal;
using Wipcore.Enova.Api.Abstractions.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace Wipcore.Enova.Api.OAuth
{
    /// <summary>
    /// This controller is used to login customers and admins into Enova. It should put a cookie and token in the response, which should be sent back by the client on subsequent requests. 
    /// </summary>
    [Route("[controller]")]
    public class AccountController : EnovaApiController
    {
        private readonly IAuthService _authService;

        public AccountController(IAuthService authService, IExceptionService exceptionService) : base(exceptionService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Get information about any logged in user.
        /// </summary>
        [HttpGet("LoggedInAs")]
        [AllowAnonymous]
        public IDictionary<string, object> LoggedInAs()
        {
            return _authService.GetLoggedInAlias() == null ?
                new Dictionary<string, object>() { { "LoggedIn", false } } :
                new Dictionary<string, object>()
                {
                    { "LoggedIn", true }, { "Alias", _authService.GetLoggedInAlias() }, { "Identifier", _authService.GetLoggedInIdentifier() }, { "Name", _authService.GetLoggedInName() },
                    { "LoginTime", _authService.GetClaim("auth_time") },  { "Role", _authService.GetLoggedInRole() }, { "Id", _authService.GetLoggedInId() },
                    { "Currency", _authService.GetLoggedInDefaultCurrency() }, { "Language", _authService.GetLoggedInDefaultLanguage() }
                };
        }

        /// <summary>
        /// Logout from Enova.
        /// </summary>
        /// <returns></returns>
        [HttpPost("Logout")]
        [AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();

            return Ok("Successful logout.");
        }

        /// <summary>
        /// Login as an administrator.
        /// </summary>
        [HttpPost("LoginAdmin")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginAdmin([FromBody]LoginModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new LoginResponseModel("Alias and password required."));

            
            var claimsPrincipal = _authService.Login(model, admin: true);

            if (claimsPrincipal == null)
                return BadRequest(new LoginResponseModel("Invalid alias or password."));

            await HttpContext.SignInAsync(claimsPrincipal);
            
            var bearerToken = _authService.BuildToken(claimsPrincipal);
            var contextModel = new ContextModel() {Currency = claimsPrincipal.FindFirst("currency")?.Value, Language = claimsPrincipal.FindFirst("language")?.Value };

            return Ok(new LoginResponseModel("Successful login.", claimsPrincipal.FindFirst(AuthService.IdentifierClaim).Value, 
                Convert.ToInt32(claimsPrincipal.FindFirst(AuthService.IdClaim).Value), bearerToken, contextModel));
        }

        /// <summary>
        /// Login as a customer.
        /// </summary>
        [HttpPost("LoginCustomer")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginCustomer([FromBody]LoginModel model)
        {
            if (!ModelState.IsValid)
                return (BadRequest(new LoginResponseModel("Alias and password required.")));

            var claimsPrincipal = _authService.Login(model, admin: false);

            if (claimsPrincipal == null)
                return BadRequest(new LoginResponseModel("Invalid alias or password."));

            await HttpContext.SignInAsync(claimsPrincipal);

            var bearerToken = _authService.BuildToken(claimsPrincipal);
            var contextModel = new ContextModel() { Currency = claimsPrincipal.FindFirst("currency")?.Value, Language = claimsPrincipal.FindFirst("language")?.Value };

            return Ok(new LoginResponseModel("Successful login.", claimsPrincipal.FindFirst(AuthService.IdentifierClaim).Value,
                Convert.ToInt32(claimsPrincipal.FindFirst(AuthService.IdClaim).Value), bearerToken, contextModel));
        }

        /// <summary>
        /// Login as a customer by providing a customer identifier, and the alias and password of an administrator.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("LoginCustomerWithAdminCredentials")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginCustomerWithAdminCredentials([FromBody]LoginCustomerWithAdminCredentialsModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new LoginResponseModel("Alias and password required."));

            var claimsPrincipal = _authService.LoginCustomerAsAdmin(model, out var errorMessage);

            if (claimsPrincipal == null)
                return BadRequest(new LoginResponseModel(errorMessage));

            await HttpContext.SignInAsync(claimsPrincipal);

            var bearerToken = _authService.BuildToken(claimsPrincipal);
            var contextModel = new ContextModel() { Currency = claimsPrincipal.FindFirst("currency")?.Value, Language = claimsPrincipal.FindFirst("language")?.Value };


            return Ok(new LoginResponseModel("Successful login.", claimsPrincipal.FindFirst(AuthService.IdentifierClaim).Value,
                Convert.ToInt32(claimsPrincipal.FindFirst(AuthService.IdClaim).Value), bearerToken, contextModel));
        }
    }
}
