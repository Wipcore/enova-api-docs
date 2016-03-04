using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wipcore.Enova.Api.Interfaces;

namespace Wipcore.Enova.Api.Models
{
    public class ContextModel : IContextModel
    {
        public string Market { get; set; } = "default";

        public string Language { get; set; }

        public string Currency { get; set; }

        public string Customer { get; set; }

        public string Admin { get; set; }

        public string Pass { get; set; }
    }
}
