using System;
using System.Collections.Generic;
using Wipcore.Core.SessionObjects;

namespace Wipcore.Enova.Api.Abstractions.Interfaces
{
    public interface IMappingToEnovaService
    {
        /// <summary>
        /// Maps given properties in dictionary to the given enova object. Returns mappers that must be set after object is saved.
        /// </summary>
        List<Action> MapToEnovaObject(BaseObject obj, IDictionary<string, object> values, List<Action> delayedMappers = null);
    }
}
