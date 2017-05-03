using System;
using System.Collections.Generic;
using Wipcore.Core.SessionObjects;

namespace Wipcore.Enova.Api.Abstractions.Interfaces
{
    public interface IAttributeService
    {
        /// <summary>
        /// Get attributes of the object with the given identifier.
        /// </summary>
        BaseObjectList GetAttributes<T>(string identifier) where T : BaseObject;

        /// <summary>
        /// Get all objects that has an attributevalue.
        /// </summary>
        IEnumerable<IDictionary<string, object>> GetObjectsWithAttributeValue(int attributeValueId);
    }
}