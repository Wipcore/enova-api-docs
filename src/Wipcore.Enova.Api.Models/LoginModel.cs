using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Wipcore.Enova.Api.Models.Interfaces;
using System;
using System.Threading.Tasks;

namespace Wipcore.Enova.Api.Models
{
    /// <summary>
    /// Model for logging in.
    /// </summary>
    public class LoginModel : ILoginModel
    {
        [Required]
        public string Alias { get; set; }
        [Required]
        public string Password { get; set; }
        public override string ToString() => $"LoginModel: (Alias={Alias}, Password=****)";

    }

    /// <summary>
    /// Model for logging in a customer with the alias and password of an administrator.
    /// </summary>
    public class LoginCustomerWithAdminCredentialsModel : LoginModel, ILoginCustomerWithAdminCredentialsModel
    {
        [Required]
        public string CustomerIdentifier { get; set; }

        public override string ToString() => $"LoginModel: (Alias={Alias}, Password=****, CustomerIdentifier={CustomerIdentifier})";
    }

    /// <summary>
    /// Response model for an loggin-request.
    /// </summary>
    public class LoginResponseModel : ILoginResponseModel
    {
        /// <summary>
        /// Response message, success or failure.
        /// </summary>
        [Required]
        public string StatusMessage { get; set; }

        /// <summary>
        /// Identifier of the customer/admin that logged in.
        /// </summary>
        public string UserIdentifier { get; set; }

        /// <summary>
        /// ID of the customer/admin that logged in.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Default language/currency context for the user.
        /// </summary>
        public ContextModel ContextModel { get; set; }

        /// <summary>
        /// Bearer authentication access token.
        /// </summary>
        public string AccessToken { get; set; }
        
        public LoginResponseModel(string message, string identifier = null, string id = null, string accessToken = null, ContextModel contextModel = null)
        {
            StatusMessage = message;
            ContextModel = contextModel;
            UserIdentifier = identifier;
            AccessToken = accessToken;
            UserId = id;
        }

        public override string ToString()
        {
            return StatusMessage;
        }
    }
}
