using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Models.Interfaces;

namespace Wipcore.Enova.Api.Interfaces
{
    /// <summary>
    /// Service for getting and saving objects to Enova.
    /// </summary>
    public interface IObjectService
    {
        /// <summary>
        /// Get objects from Enova. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestContext">Context for the query, ie language.</param>
        /// <param name="query">Query parameters.</param>
        /// <param name="candidates">Objects to look at, or null to look at all objects.</param>
        /// <returns></returns>
        IEnumerable<IDictionary<string, object>> Get<T>(IContextModel requestContext, IQueryModel query, BaseObjectList candidates = null) where T : BaseObject;

        /// <summary>
        /// Get an objects from Enova. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestContext">Context for the query, ie language.</param>
        /// <param name="query">Query parameters.</param>
        /// <param name="identifier">Object identifier.</param>
        /// <returns></returns>
        IDictionary<string, object> Get<T>(IContextModel requestContext, IQueryModel query, string identifier) where T : BaseObject;

        /// <summary>
        /// Save an object to Enova with the given values.
        /// </summary>
        /// <typeparam name="T">The most derived type of T is saved.</typeparam>
        /// <param name="requestContext">Context for the query, ie language.</param>
        /// <param name="values">Properties to save on the object.</param>
        /// <returns></returns>
        IDictionary<string, object> Save<T>(IContextModel requestContext, Dictionary<string, object> values) where T : BaseObject; 
    }
}
