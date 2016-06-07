using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Interfaces;
using Wipcore.Enova.Generics;


namespace Wipcore.Enova.Api.WebApi.Services
{
    /// <summary>
    /// Handles sorting in responses with many objects.
    /// </summary>
    public class SortService : ISortService
    {
        /// <summary>
        /// Sort the given objects by the given sort string.
        /// </summary>
        public BaseObjectList Sort(BaseObjectList objects, string sort)
        {
            if (String.IsNullOrEmpty(sort))
                return objects;

            sort = sort.Replace(',', ';');
            objects.Sort(sort);
            return objects;
        }
    }
}
