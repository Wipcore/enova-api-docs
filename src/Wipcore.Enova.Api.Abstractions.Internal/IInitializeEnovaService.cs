using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wipcore.Core.SessionObjects;

namespace Wipcore.Enova.Api.Abstractions.Internal
{
    /// <summary>
    /// This service seeds default Enova data into a new database.
    /// </summary>
    public interface IInitializeEnovaService
    {
        /// <summary>
        /// Run Enova initializer. If no context given, a new one will be created on the systemfacade. If forcerun is true it will bypass settings to turn it off.
        /// </summary>
        void InitializeEnova(Context context = null, bool forceRun = false);
    }
}
