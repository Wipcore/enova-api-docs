using System.Collections.Generic;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Abstractions.Interfaces;

namespace Wipcore.Enova.Api.Abstractions.Internal
{
    /// <summary>
    /// Service for getting, deleting and saving objects to Enova.
    /// </summary>
    public interface IObjectService
    {
        /// <summary>
        /// Returns true if object with identifier was found.
        /// </summary>
        bool Exists<T>(string identifier);

        /// <summary>
        /// Returns true if object with id was found.
        /// </summary>
        bool Exists<T>(int id);
        
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
        /// Get an objects from Enova. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestContext">Context for the query, ie language.</param>
        /// <param name="query">Query parameters.</param>
        /// <param name="id">Object id.</param>
        /// <returns></returns>
        IDictionary<string, object> Get<T>(IContextModel requestContext, IQueryModel query, int id) where T : BaseObject;


        /// <summary>
        /// Get objects from Enova. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestContext">Context for the query, ie language.</param>
        /// <param name="query">Query parameters.</param>
        /// <param name="candidates">Objects to look at, or null to look at all objects.</param>
        /// <returns></returns>
        IEnumerable<IDictionary<string, object>> GetMany<T>(IContextModel requestContext, IQueryModel query, BaseObjectList candidates = null) where T : BaseObject;

        /// <summary>
        /// Get several objects from Enova.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestContext">Context for the query, ie language.</param>
        /// <param name="query">Query parameters.</param>
        /// <param name="ids">List of ids of objects to find.</param>
        IEnumerable<IDictionary<string, object>> GetMany<T>(IContextModel requestContext, IQueryModel query, IEnumerable<int> ids) where T : BaseObject;

        /// <summary>
        /// Get several objects from Enova.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestContext">Context for the query, ie language.</param>
        /// <param name="query">Query parameters.</param>
        /// <param name="identifiers">List of identifiers of objects to find.</param>
        IEnumerable<IDictionary<string, object>> GetMany<T>(IContextModel requestContext, IQueryModel query, IEnumerable<string> identifiers) where T : BaseObject;


        /// <summary>
        /// Save an object to Enova with the given values.
        /// </summary>
        /// <typeparam name="T">The most derived type of T is saved.</typeparam>
        /// <param name="requestContext">Context for the query, ie language.</param>
        /// <param name="values">Properties to save on the object.</param>
        /// <returns></returns>
        IDictionary<string, object> Save<T>(IContextModel requestContext, Dictionary<string, object> values) where T : BaseObject;


        /// <summary>
        /// Calculate the effects of mapping an object without actually saving it. Useful for carts.
        /// </summary>
        /// <typeparam name="T">The most derived type of T is calculated.</typeparam>
        /// <param name="requestContext">Context for the query, ie language.</param>
        /// <param name="values">Properties to calculate on the object.</param>
        IDictionary<string, object> Calculate<T>(IContextModel requestContext, Dictionary<string, object> values) where T : BaseObject;

        /// <summary>
        /// Deletes an object of type T and given id from Enova. Returns true if successfull.
        /// </summary>
        bool Delete<T>(int id) where T : BaseObject;

        /// <summary>
        /// Deletes an object of type T and given identifier from Enova. Returns true if successfull. False if the object did not exist.
        /// </summary>
        bool Delete<T>(string identifier) where T : BaseObject;
    }
}
