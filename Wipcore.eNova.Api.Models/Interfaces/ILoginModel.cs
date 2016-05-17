namespace Wipcore.Enova.Api.Models.Interfaces
{
    public interface ILoginModel
    {
        string Password { get; set; }
        string Username { get; set; }
        
    }

    public interface ILoginCustomerWithAdminCredentialsModel : ILoginModel
    {
        string CustomerIdentifier { get; set; }
    }
}