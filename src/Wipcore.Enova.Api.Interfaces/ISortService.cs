using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wipcore.Core.SessionObjects;

namespace Wipcore.Enova.Api.Interfaces
{
    /// <summary>
    /// Handles sorting in responses with many objects.
    /// </summary>
    public interface ISortService
    {
        /// <summary>
        /// Sort the given objects by the given sort string.
        /// </summary>
        BaseObjectList Sort(BaseObjectList objects, string sort);
    }
}
