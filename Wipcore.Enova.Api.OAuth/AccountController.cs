using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using IdentityModel;
using IdentityServer4.Core;
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
        /// Logout from Enova.
        /// </summary>
        /// <returns></returns>
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.Authentication.SignOutAsync(Constants.PrimaryAuthenticationType);
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
                return HttpBadRequest("Username and password required.");

            var claimsPrincipal = _loginService.Login(model, admin:true);

            if (claimsPrincipal == null)
                return HttpBadRequest("Invalid username or password.");

            await HttpContext.Authentication.SignInAsync(Constants.PrimaryAuthenticationType, claimsPrincipal);
            
            return Ok("Successful login.");
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
                return HttpBadRequest("Username and password required.");

            var claimsPrincipal = _loginService.Login(model, admin: false);

            if (claimsPrincipal == null)
                return HttpBadRequest("Invalid username or password.");

            await HttpContext.Authentication.SignInAsync(Constants.PrimaryAuthenticationType, claimsPrincipal);

            return Ok("Successful login.");
        }

        /// <summary>
        /// Login as a customer by providing a customer identifier, and the username and password of an administrator.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("LoginCustomerWithAdminCredentials")]
        public async Task<IActionResult> LoginCustomerWithAdminCredentials([FromBody]LoginCustomerWithAdminCredentialsModel model)
        {
            if (!ModelState.IsValid)
                return HttpBadRequest("Username and password required.");

            var claimsPrincipal = _loginService.LoginCustomerAsAdmin(model);

            if (claimsPrincipal == null)
                return HttpBadRequest("Invalid username or password.");

            await HttpContext.Authentication.SignInAsync(Constants.PrimaryAuthenticationType, claimsPrincipal);

            return Ok("Successful login.");
        }
    }
}
