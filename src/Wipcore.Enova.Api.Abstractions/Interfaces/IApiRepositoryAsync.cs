using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Wipcore.Enova.Api.Abstractions.Models;
using Wipcore.Enova.Api.Abstractions.Models.EnovaTypes;

namespace Wipcore.Enova.Api.Abstractions.Interfaces
{
    /// <summary>
    /// A repository for async access to Enova through the API. 
    /// </summary>
    public interface IApiRepositoryAsync
    {
        /// <summary>
        /// Check if object with given identifier exists.
        /// </summary>
        Task<bool> ObjectExistsAsync<TModel>(string identifier) where TModel : BaseModel;

        /// <summary>
        /// Check if object with given id exists.
        /// </summary>
        Task<bool> ObjectExistsAsync<TModel>(int id) where TModel : BaseModel;

        /// <summary>
        /// Get an object by id, serialized into TModel.
        /// </summary>
        Task<TModel> GetObjectAsync<TModel>(int id, QueryModel queryModel = null, ContextModel contextModel = null, string action = null, IDictionary<string, object> extraParameters = null) where TModel : BaseModel;

        /// <summary>
        /// Get an object by identifier, seralized into TModel. 
        /// </summary>
        Task<TModel> GetObjectAsync<TModel>(string identifier, QueryModel queryModel = null, ContextModel contextModel = null, string action = null, IDictionary<string, object> extraParameters = null) where TModel : BaseModel;

        /// <summary>
        /// Get many objects, serialized into TModel. Query parameters, outputheaders, context/culture and which languages to query for are all optional.
        /// </summary>
        Task<IEnumerable<object>> GetManyAsync(Type modelType, QueryModel queryModel = null, ApiResponseHeadersModel headers = null, ContextModel contextModel = null, string action = null, List<string> languages = null, IDictionary<string, object> extraParameters = null);

        /// <summary>
        /// Get many objects, untyped. Query parameters, outputheaders, context/culture and which languages to query for are all optional.
        /// </summary>
        Task<IEnumerable<TModel>> GetManyAsync<TModel>(QueryModel queryModel = null, ApiResponseHeadersModel headers = null, ContextModel contextModel = null, string action = null, List<string> languages = null, IDictionary<string, object> extraParameters = null);

        /// <summary>
        /// Get the next page of items from a previous request.
        /// </summary>
        Task<IEnumerable<TModel>> GetNextPageAsync<TModel>(ApiResponseHeadersModel headersOfPreviousRequest);

        /// <summary>
        /// Get the previous page of items from a previous request.
        /// </summary>
        Task<IEnumerable<TModel>> GetPreviousPageAsync<TModel>(ApiResponseHeadersModel headersOfPreviousRequest);

        /// <summary>
        /// Deletes an object by id.
        /// </summary>
        Task<bool> DeleteObjectAsync<TModel>(int id) where TModel : BaseModel;

        /// <summary>
        /// Deletes an object by identifier.
        /// </summary>
        Task<bool> DeleteObjectAsync<TModel>(string identifier) where TModel : BaseModel;

        /// <summary>
        /// Saves an object. Specify specific action in url, responsetype to serialize the response into, and context/culture, as needed.
        /// </summary>
        Task<object> SaveObjectAsync<TModel>(JObject item, string action = null, Type responseType = null, ContextModel contextModel = null, bool verifyIdentifierNotTaken = true, IDictionary<string, object> extraParameters = null) where TModel : BaseModel;
    }
}
