namespace Wipcore.Enova.Api.Models.Interfaces
{
    public interface IGetParametersModel
    {
        string Template { get; set; }
        string Properties { get; set; }
        int? Page { get; set; }
        int? Size { get; set; }
        string Sort { get; set; }
        string Filter { get; set; }
    }
}