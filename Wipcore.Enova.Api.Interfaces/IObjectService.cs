using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Models.Interfaces;

namespace Wipcore.Enova.Api.Interfaces
{
    public interface IObjectService
    {
        IEnumerable<IDictionary<string, object>> Get<T>(IContextModel requestContext, IGetParametersModel getParameters, BaseObjectList candidates = null) where T : BaseObject;

        IDictionary<string, object> Get<T>(IContextModel requestContext, IGetParametersModel getParameters, string identifier) where T : BaseObject;

        //TODO different interface for save?
        IDictionary<string, object> Save<T>(IContextModel requestContext, Dictionary<string, object> values) where T : BaseObject; 
    }
}
