using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Wipcore.Core.SessionObjects;

namespace Wipcore.Enova.Api.WebApi
{
    public static class EnovaContextProvider
    {
        

        

        public static Func<Context> GetCurrentContext = () =>
        {
            return EnovaSystemFacade.Current.Connection.Context;
        };

        //public static Context CreateNewContext => EnovaSystemFacade.Current.Connection.CreateContext();

        private static Context GetOrCreateContext()
        {
            

            return null;
        }
    }
}
