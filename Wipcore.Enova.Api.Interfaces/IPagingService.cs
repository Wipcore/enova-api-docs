using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wipcore.Core.SessionObjects;


namespace Wipcore.Enova.Api.Interfaces
{
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

    }
}
