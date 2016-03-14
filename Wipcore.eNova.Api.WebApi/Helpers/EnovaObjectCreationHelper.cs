using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wipcore.Core.SessionObjects;

namespace Wipcore.Enova.Api.WebApi.Helpers
{
    public static class EnovaObjectCreationHelper
    {

        public static T CreateNew<T>(Context context, params object[] args) where T : BaseObject
        {
            var type = typeof (T).GetMostDerivedType();
            var allArgs = new[] {context}.Union(args).ToArray();
            var item = (T)Activator.CreateInstance(type, allArgs);
            return item;
        }
    }
}
