using System;
using System.IO;
using System.Reflection;
using Wipcore.Enova.Connectivity;

namespace Wipcore.Enova.Api.OAuth
{
    /// <summary>
    /// Provides an easy way to start up an Enova system.
    /// </summary>
    public class EnovaSystemFacade
    {
        public InMemoryConnectionSettings Settings { get; set; }

        /// <summary>
        /// The Enova connection
        /// Use the set only in unit tests
        /// </summary>
        public InMemoryConnection Connection { get; set; }

        /// <summary>
        /// Get the singleton instance of the Enova system facade. You can use this
        /// property in cases where you cannot use constructor injection.
        /// </summary>
        public static EnovaSystemFacade Current { get; } = new EnovaSystemFacade();


        /// <summary>
        /// Starts the Enova system.
        /// </summary>
        public void Start()
        {
            Connection = new InMemoryConnection(Settings);
        }

        public void Stop()
        {
            Dispose();
        }

        public void Dispose()
        {
            Connection?.Dispose();
            Connection = null;
        }
    }
}