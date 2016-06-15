namespace Wipcore.Enova.Api.Models.Interfaces
{
    /// <summary>
    /// Parameters for handling a get request.
    /// </summary>
    public interface IQueryModel
    {
        /// <summary>
        /// Identifier of pre-specified settings for request.
        /// </summary>
        string Template { get; set; }
        /// <summary>
        /// Comma-seperated list of the properties to map.
        /// </summary>
        string Properties { get; set; }
        /// <summary>
        /// Pagination. Index of page to retrieve. 
        /// </summary>
        int? Page { get; set; }
        /// <summary>
        /// Size of the page; the number of items to retrieve.
        /// </summary>
        int? Size { get; set; }
        /// <summary>
        /// Sort order. Example: Identifier,Name Desc
        /// </summary>
        string Sort { get; set; }
        /// <summary>
        /// Filter the response. Example: Identifier=123 OR Name=pizza*
        /// </summary>
        string Filter { get; set; }
    }
}