using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wipcore.Core.SessionObjects;

namespace Wipcore.Enova.Api.Interfaces
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
