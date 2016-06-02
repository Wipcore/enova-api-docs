using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wipcore.Core.SessionObjects;

namespace Wipcore.Enova.Api.Interfaces
{
    public interface IMappingToEnovaService
    {
        /// <summary>
        /// Maps given properties in dictionary to the given enova object.
        /// </summary>
        IDictionary<string, object> MapToEnovaObject(BaseObject obj, IDictionary<string, object> values);
    }
}
