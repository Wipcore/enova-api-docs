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

        /// <summary>
        /// Identifier of any customer for whom the request shall be processed.
        /// </summary>
        public string Customer { get; set; }

        /// <summary>
        /// Identifier of an admin. Required to access certain elements.
        /// </summary>
        public string Admin { get; set; }

        /// <summary>
        /// Password of the admin.
        /// </summary>
        public string Pass { get; set; }

        public override string ToString()
        {
            var pass = Pass == null ? null : "****";
            return $"ContextModel: (Market: {Market}, Language: {Language}, Currency: {Currency}, Customer: {Customer}, Admin: {Admin}, Pass: {pass})";
        }
    }
}
