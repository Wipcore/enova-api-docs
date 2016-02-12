using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wipcore.Enova.Api.Interfaces;

namespace Wipcore.Enova.Api.WebApi.Models
{
    public class GetParametersModel : IGetParametersModel
    {
        public string Location { get; set; } = "default";

        public string Properties { get; set; }

        public int? Page { get; set; }

        public int? Size { get; set; }

        public string Sort { get; set; } 

        public string Filter { get; set; }
        
    }
}
