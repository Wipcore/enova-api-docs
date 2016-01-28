using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wipcore.Enova.Api.Interfaces;

namespace Wipcore.eNova.Api.WebApi.Models
{
    public class ContextModel : IContextModel
    {
        public string Market { get; set; }

        public string Language { get; set; }

        public string Currency { get; set; }

        public string User { get; set; }
    }
}
