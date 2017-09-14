using System.Collections.Generic;
using Wipcore.Core.SessionObjects;

namespace Wipcore.Enova.Api.Abstractions.Internal
{
    /// <summary>
    /// Handles pagination of responses with many objects.
    /// </summary>
    public interface IPagingService
    {
        /// <summary>
        /// Get a subset/page of a list of objects.
        /// </summary>
        /// <param name="objects">Candidate objects to page.</param>
        /// <param name="pageNumber">The page numer, i.e. page 2 to view objects 20-40 if pagesize is 20.</param>
        /// <param name="pageSize">The number of objects to get per page.</param>
        /// <returns></returns>
        BaseObjectList Page(BaseObjectList objects, int pageNumber, int pageSize);

        /// <summary>
        /// Get all response headers.
        /// </summary>
        IDictionary<string, string> GetHeaders();

        /// <summary>
        /// Set given values into the response headers.
        /// </summary>
        void SetHeaders(IDictionary<string, string> headers);

    }
}
