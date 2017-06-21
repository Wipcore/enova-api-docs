using Wipcore.Core.SessionObjects;

namespace Wipcore.Enova.Api.Abstractions.Internal
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
