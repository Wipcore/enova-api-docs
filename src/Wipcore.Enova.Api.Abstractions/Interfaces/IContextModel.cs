namespace Wipcore.Enova.Api.Abstractions.Interfaces
{
    /// <summary>
    /// Context for requests, specifying culture settings.
    /// </summary>
    public interface IContextModel
    {
        /// <summary>
        /// Name of a pre-defined market read from configuration.
        /// </summary>
        string Market { get; set; }

        /// <summary>
        /// Identifier of the Enova language to use during the request.
        /// </summary>
        string Language { get; set; }

        /// <summary>
        /// Identifier of the Enova currency used during the request.
        /// </summary>
        string Currency { get; set; }

        /// <summary>
        /// Seperator between thousands in currency strings.
        /// </summary>
        string ThousandSeparator { get; set; }

        /// <summary>
        /// Seperator for decimals in currency strings.
        /// </summary>
        string DecimalSeparator { get; set; }
    }
}