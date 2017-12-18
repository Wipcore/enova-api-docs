using System;
using System.Collections.Generic;
using Wipcore.Core.SessionObjects;

namespace Wipcore.Enova.Api.Abstractions.Internal
{
    public interface IMappingToEnovaService
    {
        /// <summary>
        /// Maps given properties in dictionary to the given enova object. Returns mappers that must be set after object is saved.
        /// </summary>
        List<Action> MapToEnovaObject(BaseObject obj, IDictionary<string, object> values, List<Action> delayedMappers = null);

        /// <summary>
        /// Clear the cache of a property that has been removed.
        /// </summary>
        void ClearSettablePropertyCache(Type type, string propertyName);
    }
}
