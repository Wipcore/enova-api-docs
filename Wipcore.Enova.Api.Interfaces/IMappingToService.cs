using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wipcore.Core.SessionObjects;

namespace Wipcore.Enova.Api.Interfaces
{
    public interface IMappingToService
    {
        IDictionary<string, object> MapToEnovaObject(BaseObject obj, IDictionary<string, object> values);
    }
}
