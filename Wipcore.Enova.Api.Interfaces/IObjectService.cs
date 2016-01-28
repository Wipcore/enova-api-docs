using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wipcore.Core.SessionObjects;

namespace Wipcore.Enova.Api.Interfaces
{
    public interface IObjectService
    {
        IEnumerable<IDictionary<string, object>> Get<T>(IContextModel requestContext, IGetParametersModel getParameters) where T : BaseObject;

        IDictionary<string, object> Get<T>(string identifier, string properties = null) where T : BaseObject;
    }
}
