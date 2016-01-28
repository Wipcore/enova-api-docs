namespace Wipcore.Enova.Api.Interfaces
{
    public interface IGetParametersModel
    {
        string Location { get; set; }
        string Properties { get; set; }
        int? Page { get; set; }
        int? Size { get; set; }
        string Sort { get; set; }
        string Filter { get; set; }
    }
}