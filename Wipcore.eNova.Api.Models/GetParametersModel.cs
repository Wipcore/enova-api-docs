using Wipcore.Enova.Api.Models.Interfaces;

namespace Wipcore.Enova.Api.Models
{
    public class GetParametersModel : IGetParametersModel
    {
        public string Location { get; set; } = "default";

        public string Properties { get; set; }

        public int? Page { get; set; }

        public int? Size { get; set; }

        public string Sort { get; set; } 

        public string Filter { get; set; }

        public override string ToString()
        {
            return $"GetParametersModel: (Location: {Location}, Properties: {Properties}, Page: {Page}, Size: {Size}, Sort: {Sort}, Filter: {Filter})";
        }
    }
}
