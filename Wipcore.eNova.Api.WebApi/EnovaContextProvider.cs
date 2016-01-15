using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wipcore.Core.SessionObjects;

namespace Wipcore.eNova.Api.WebApi
{
    public static class EnovaContextProvider
    {
        public static Func<Context> GetCurrentContext = () =>
        {
            return EnovaSystemFacade.Current.Connection.Context;
        };
    }
}
