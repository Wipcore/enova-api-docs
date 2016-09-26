using System;
using System.Text;
using Wipcore.Enova.Api.Models.Interfaces;

namespace Wipcore.Enova.Api.Models
{
    public class ContextModel : IContextModel
    {
        /// <summary>
        /// Identifier of pre-defined market.
        /// </summary>
        public string Market { get; set; } = "default";

        /// <summary>
        /// Identifier of language to use.
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Identifier of currency to use.
        /// </summary>
        public string Currency { get; set; }

        public override string ToString()
        {
            return $"ContextModel: (Market: {Market}, Language: {Language}, Currency: {Currency})";
        }
    }
}
