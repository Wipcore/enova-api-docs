using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wipcore.Core.SessionObjects;


namespace Wipcore.eNova.Api.Interfaces
{
    public interface IPagingService
    {
        BaseObjectList Page(BaseObjectList objects, int pageNumber, int pageSize);

    }
}
