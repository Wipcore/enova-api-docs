using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Wipcore.Enova.Api.Abstractions.Models;
using Wipcore.Enova.Api.Abstractions.Models.EnovaTypes;

namespace Wipcore.Enova.Api.Abstractions.Interfaces
{
    /// <summary>
    /// A repository for access to Enova through the API. 
    /// </summary>
    public interface IApiRepository
    {
        /// <summary>
        /// Check if object with given identifier exists.
        /// </summary>
        bool ObjectExists<TModel>(string identifier) where TModel : BaseModel;

        /// <summary>
        /// Check if object with given id exists.
        /// </summary>
        bool ObjectExists<TModel>(int id) where TModel : BaseModel;

        /// <summary>
        /// Get an object by id, serialized into TModel.
        /// </summary>
        TModel GetObject<TModel>(int id, QueryModel queryModel = null, ContextModel contextModel = null, string action = null, IDictionary<string, object> extraParameters = null) where TModel : BaseModel;

        /// <summary>
        /// Get an object by identifier, seralized into TModel. 
        /// </summary>
        TModel GetObject<TModel>(string identifier, QueryModel queryModel = null, ContextModel contextModel = null, string action = null, IDictionary<string, object> extraParameters = null) where TModel : BaseModel;

        /// <summary>
        /// Get many objects, serialized into TModel. Query parameters, outputheaders, context/culture and which languages to query for are all optional.
        /// </summary>
        IEnumerable<object> GetMany(Type modelType, QueryModel queryModel = null, ApiResponseHeadersModel headers = null, ContextModel contextModel = null, string action = null, List<string> languages = null, IDictionary<string, object> extraParameters = null);

        /// <summary>
        /// Get many objects, untyped. Query parameters, outputheaders, context/culture and which languages to query for are all optional.
        /// </summary>
        IEnumerable<TModel> GetMany<TModel>(QueryModel queryModel = null, ApiResponseHeadersModel headers = null, ContextModel contextModel = null, string action = null, List<string> languages = null, IDictionary<string, object> extraParameters = null);

        /// <summary>
        /// Deletes an object by id.
        /// </summary>
        bool DeleteObject<TModel>(int id) where TModel : BaseModel;

        /// <summary>
        /// Deletes an object by identifier.
        /// </summary>
        bool DeleteObject<TModel>(string identifier) where TModel : BaseModel;

        /// <summary>
        /// Saves an object. Specify specific action in url, responsetype to serialize the response into, and context/culture, as needed.
        /// </summary>
        object SaveObject<TModel>(JObject item, string action = null, Type responseType = null, ContextModel contextModel = null, bool verifyIdentifierNotTaken = true, IDictionary<string, object> extraParameters = null) where TModel : BaseModel;
    }
}