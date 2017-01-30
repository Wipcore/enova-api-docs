using Wipcore.Enova.Api.Abstractions.Interfaces;

namespace Wipcore.Enova.Api.Abstractions.Models
{
    public class QueryModel : IQueryModel
    {
        /// <summary>
        /// Identifier of pre-specified settings for request.
        /// </summary>
        public string Template { get; set; } = "default";

        /// <summary>
        /// Comma-seperated list of the properties to map.
        /// </summary>
        public string Properties { get; set; }

        /// <summary>
        /// Pagination. Index of page to retrieve. 
        /// </summary>
        public int? Page { get; set; }

        /// <summary>
        /// Size of the page; the number of items to retrieve.
        /// </summary>
        public int? Size { get; set; }

        /// <summary>
        /// Sort order. Example: Identifier,Name Desc
        /// </summary>
        public string Sort { get; set; } 

        /// <summary>
        /// Filter the response. Example: Identifier=123 OR Name=pizza*
        /// </summary>
        public string Filter { get; set; }

        public override string ToString()
        {
            return $"QueryModel: (Template: {Template}, Properties: {Properties}, Page: {Page}, Size: {Size}, Sort: {Sort}, Filter: {Filter})";
        }

        /// <summary>
        /// Creates a copy of the model.
        /// </summary>
        /// <returns></returns>
        public QueryModel Copy()
        {
            return new QueryModel() { Template = Template, Properties = Properties, Filter = Filter, Size = Size, Sort = Sort, Page = Page};
        }
    }
}
