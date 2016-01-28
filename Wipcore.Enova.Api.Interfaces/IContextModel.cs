namespace Wipcore.Enova.Api.Interfaces
{
    public interface IContextModel
    {
        string Market { get; set; }
        string Language { get; set; }
        string Currency { get; set; }
        string User { get; set; }
    }
}