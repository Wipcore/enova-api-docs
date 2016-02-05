using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wipcore.Core.SessionObjects;

namespace Wipcore.eNova.Api.WebApi.Helpers
{
    public static class EnovaObjectCreationHelper
    {

        public static T CreateNew<T>(Context context) where T : BaseObject
        {
            var type = typeof (T).GetMostDerivedType();
            var item = (T)Activator.CreateInstance(type, context);

            return item;
        }
    }
}
