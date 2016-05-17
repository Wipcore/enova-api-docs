namespace Wipcore.Enova.Api.Models.Interfaces
{
    public interface IContextModel
    {
        string Market { get; set; }
        string Language { get; set; }
        string Currency { get; set; }
    }
}