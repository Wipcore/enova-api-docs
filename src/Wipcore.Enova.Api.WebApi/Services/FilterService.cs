using System;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Abstractions.Interfaces;
using Wipcore.Enova.Api.Abstractions.Internal;

namespace Wipcore.Enova.Api.WebApi.Services
{
    public class FilterService : IFilterService
    {
        /// <summary>
        /// Filter the given objects by the given filter string.
        /// </summary>
        /// <param name="objects"></param>
        /// <param name="filter">I.E: Name=myfinename</param>
        /// <returns></returns>
        public BaseObjectList Filter(BaseObjectList objects, string filter)
        {
            if (String.IsNullOrEmpty(filter))
                return objects;

            var filteredObjects = objects.Search(filter);
            return filteredObjects;
        }
    }
}
