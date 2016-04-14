using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wipcore.Core.SessionObjects;

namespace Wipcore.Enova.Api.Interfaces
{
    public interface IMappingFromService
    {
        IEnumerable<IDictionary<string, object>> MapFromEnovaObject(BaseObjectList objects, string properties);

        IDictionary<string, object> MapFromEnovaObject(BaseObject obj, string properties);
    }
}
