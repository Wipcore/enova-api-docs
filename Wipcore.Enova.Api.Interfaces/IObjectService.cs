using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wipcore.Core.SessionObjects;

namespace Wipcore.eNova.Api.Interfaces
{
    public interface IObjectService
    {
        IEnumerable<IDictionary<string, object>> Get<T>(int pageNumber, int pageSize, string properties, string sort, string filter) where T : BaseObject;

        IDictionary<string, object> Get<T>(string identifier, string properties = null) where T : BaseObject;
    }
}
