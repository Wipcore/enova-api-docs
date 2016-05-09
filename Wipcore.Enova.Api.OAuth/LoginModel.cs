using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Wipcore.Enova.Api.OAuth
{
    public class LoginModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        //public bool RememberLogin { get; set; }
        //public string SignInId { get; set; }
        public string ReturnUrl { get; set; }

        public bool IsAdmin { get; set; }

    }
}
