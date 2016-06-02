using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wipcore.Core.SessionObjects;

namespace Wipcore.Enova.Api.Interfaces
{
    public interface IMappingFromEnovaService
    {
        /// <summary>
        /// Maps given enova objects with the comma-seperated properties into dictionaries.
        /// </summary>
        IEnumerable<IDictionary<string, object>> MapFromEnovaObject(BaseObjectList objects, string properties);

        /// <summary>
        /// Maps given enova object with the comma-seperated properties into a dictionary of property-value.
        /// </summary>
        IDictionary<string, object> MapFromEnovaObject(BaseObject obj, string properties);
    }
}
