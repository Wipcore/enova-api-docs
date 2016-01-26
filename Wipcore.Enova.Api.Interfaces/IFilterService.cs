using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wipcore.Core.SessionObjects;

namespace Wipcore.Enova.Api.Interfaces
{
    public interface IFilterService
    {
        BaseObjectList Filter(BaseObjectList objects, string filter);
    }
}
