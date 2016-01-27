using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wipcore.Core.SessionObjects;

namespace Wipcore.Enova.Api.Interfaces
{
    public interface IMappingService
    {
        IEnumerable<IDictionary<string, object>> GetProperties(BaseObjectList objects, string properties);

        IDictionary<string, object> GetProperties(BaseObject obj, string properties);
    }
}
