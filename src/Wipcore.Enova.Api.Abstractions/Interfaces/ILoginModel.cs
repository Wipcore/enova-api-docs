using Wipcore.Enova.Api.Abstractions.Models;

namespace Wipcore.Enova.Api.Abstractions.Interfaces
{
    /// <summary>
    /// Model for logging in.
    /// </summary>
    public interface ILoginModel
    {
        string Password { get; set; }
        string Alias { get; set; }
        
    }

    /// <summary>
    /// Model for logging in a customer with the alias and password of an administrator.
    /// </summary>
    public interface ILoginCustomerWithAdminCredentialsModel : ILoginModel
    {
        string CustomerIdentifier { get; set; }
    }

    /// <summary>
    /// Response model for an loggin-request.
    /// </summary>
    public interface ILoginResponseModel
    {
        /// <summary>
        /// Response message, success or failure.
        /// </summary>
        string StatusMessage { get; }

        /// <summary>
        /// Identifier of the customer/admin that logged in.
        /// </summary>
        string UserIdentifier { get; }

        /// <summary>
        /// Bearer authentication access token.
        /// </summary>
        string AccessToken { get; }

        /// <summary>
        /// ID of the customer/admin that logged in.
        /// </summary>
        string UserId { get; }

        /// <summary>
        /// Default language/currency context for the user.
        /// </summary>
        ContextModel ContextModel { get; }
    }
}