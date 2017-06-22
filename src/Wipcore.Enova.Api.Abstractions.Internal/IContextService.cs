using Wipcore.Core.SessionObjects;

namespace Wipcore.Enova.Api.Abstractions.Internal
{
    public interface IContextService
    {
        /// <summary>
        /// Get an EnovaContext for the current user.
        /// </summary>
        /// <returns></returns>
        Context GetContext();
    }
}
