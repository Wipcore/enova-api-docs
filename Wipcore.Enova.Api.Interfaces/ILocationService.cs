using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wipcore.Enova.Api.Interfaces
{
    public interface ILocationService
    {
        IGetParametersModel GetParametersFromLocationConfiguration(string type, IGetParametersModel parameters);
    }
}
