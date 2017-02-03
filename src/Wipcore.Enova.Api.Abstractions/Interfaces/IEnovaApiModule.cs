using Autofac.Core;

namespace Wipcore.Enova.Api.Abstractions.Interfaces
{
    /// <summary>
    /// Autofac module to register classes in the API.
    /// </summary>
    public interface IEnovaApiModule : IModule
    {
        /// <summary>
        /// The priority for loading this module, lower priority is loaded first (which means higher priority can override previous registrations).
        /// </summary>
        int Priority { get; }
    }
}
