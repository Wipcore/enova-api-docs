using Microsoft.AspNet.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Wipcore.Enova.Api.Models.Interfaces;
using System;
using System.Threading.Tasks;

namespace Wipcore.Enova.Api.Models
{
    public class LoginModel : ILoginModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        public override string ToString() => $"LoginModel: (Username={Username}, Password=****)";

    }

    public class LoginCustomerWithAdminCredentialsModel : LoginModel, ILoginCustomerWithAdminCredentialsModel
    {
        [Required]
        public string CustomerIdentifier { get; set; }

        public override string ToString() => $"LoginModel: (Username={Username}, Password=****, CustomerIdentifier={CustomerIdentifier})";
    }

    public class LoginResponseModel : ILoginResponseModel
    {
        [Required]
        public string StatusMessage { get; }

        public string UserIdentifier { get; }

        public LoginResponseModel(string message, string identifier = null)
        {
            StatusMessage = message;
            UserIdentifier = identifier;
        }

        public override string ToString()
        {
            return StatusMessage;
        }
    }
}
