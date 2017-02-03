using Wipcore.Core.SessionObjects;

namespace Wipcore.Enova.Api.Abstractions.Interfaces
{
    public interface IContextService
    {
        /// <summary>
        /// Get an EnovaContext for the current user.
        /// </summary>
        /// <returns></returns>
        Context GetContext();
    }

    public static class ContextConstants
    {
        public const string ContextModelKey = "requestContext";
        public const string EnovaContextKey = "enovaContext";
    }
}
