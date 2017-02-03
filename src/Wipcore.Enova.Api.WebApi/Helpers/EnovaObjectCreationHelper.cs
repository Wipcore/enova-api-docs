using System;
using System.Linq;
using Wipcore.Core.SessionObjects;

namespace Wipcore.Enova.Api.WebApi.Helpers
{
    public static class EnovaObjectCreationHelper
    {
        /// <summary>
        /// Create a new Enova object of most derived type of T, with given arguments.
        /// </summary>
        public static T CreateNew<T>(Context context, params object[] args) where T : BaseObject
        {
            return (T) CreateNew(typeof (T), context, args);
        }

        /// <summary>
        /// Create a new Enova object of most derived type of t, with given arguments.
        /// </summary>
        public static object CreateNew(Type t, Context context, params object[] args)
        {
            var type = t.GetMostDerivedEnovaType();
            var allArgs = new[] { context }.Union(args).ToArray();
            var item = Activator.CreateInstance(type, allArgs);
            return item;
        }
    }
}
