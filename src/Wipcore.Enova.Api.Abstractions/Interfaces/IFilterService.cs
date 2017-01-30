using Wipcore.Core.SessionObjects;

namespace Wipcore.Enova.Api.Abstractions.Interfaces
{
    public interface IFilterService
    {
        /// <summary>
        /// Filter the given objects by the given filter string.
        /// </summary>
        /// <param name="objects"></param>
        /// <param name="filter">I.E: Name=myfinename</param>
        /// <returns></returns>
        BaseObjectList Filter(BaseObjectList objects, string filter);
    }
}
