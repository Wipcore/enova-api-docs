using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wipcore.Core.SessionObjects;

namespace Wipcore.Enova.Api.Interfaces
{
    public interface IMappingFromService
    {
        IEnumerable<IDictionary<string, object>> MapFrom(BaseObjectList objects, string properties);

        IDictionary<string, object> MapFrom(BaseObject obj, string properties);
    }
}
