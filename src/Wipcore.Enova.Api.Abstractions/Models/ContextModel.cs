using Wipcore.Enova.Api.Abstractions.Interfaces;

namespace Wipcore.Enova.Api.Abstractions.Models
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

        /// <summary>
        /// Seperator between thousands in currency strings.
        /// </summary>
        public string ThousandSeparator { get; set; }

        /// <summary>
        /// Seperator for decimals in currency strings.
        /// </summary>
        public string DecimalSeparator { get; set; }

        public override string ToString()
        {
            return $"ContextModel: (Market: {Market}, Language: {Language}, Currency: {Currency})";
        }
    }
}
