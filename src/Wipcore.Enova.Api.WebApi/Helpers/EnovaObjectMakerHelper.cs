using System;
using System.Linq;
using Fasterflect;
using Wipcore.Core;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Generics;

namespace Wipcore.Enova.Api.WebApi.Helpers
{
    public static class EnovaObjectMakerHelper
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
            var item = type.CreateInstance(allArgs);
            return item;
        }

        /// <summary>
        /// Find an Enova item, first by id then by identifier.
        /// </summary>
        public static T Find<T>(this Context context, int id, string identfier, bool throwIfNotFound = false) where T : BaseObject
        {
            var obj = context.FindObject<T>(id);
            if (obj == null && !String.IsNullOrEmpty(identfier))
                obj = context.FindObject<T>(identfier);

            if (obj == null && throwIfNotFound)
            {
                if(!String.IsNullOrEmpty(identfier))
                    throw new ObjectNotFoundException(identfier);
                throw new ObjectNotFoundException(id);
            }

            return obj;
        }
    }
}
