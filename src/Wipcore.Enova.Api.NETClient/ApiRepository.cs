using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Wipcore.Enova.Api.Abstractions.Attributes;
using Wipcore.Enova.Api.Abstractions.Interfaces;
using Wipcore.Enova.Api.Abstractions.Models;
using Wipcore.Enova.Api.Abstractions.Models.EnovaTypes;
using Fasterflect;

namespace Wipcore.Enova.Api.NetClient
{
    /// <summary>
    /// A repository for access to Enova through the API. This is a sync wrapper around ApiRepositoryAsync.
    /// </summary>
    public class ApiRepository : IApiRepository
    {
        private readonly IApiRepositoryAsync _apiRepositoryAsync;

        public ApiRepository(IApiRepositoryAsync apiRepositoryAsync)
        {
            _apiRepositoryAsync = apiRepositoryAsync;
        }

        /// <summary>
        /// Check if object with given identifier exists.
        /// </summary>
        public bool ObjectExists<TModel>(string identifier) where TModel : BaseModel => _apiRepositoryAsync.ObjectExistsAsync<TModel>(identifier).Result;

        /// <summary>
        /// Check if object with given id exists.
        /// </summary>
        public bool ObjectExists<TModel>(int id) where TModel : BaseModel => _apiRepositoryAsync.ObjectExistsAsync<TModel>(id).Result;
        
        /// <summary>
        /// Get an object by id, serialized into TModel.
        /// </summary>
        public TModel GetObject<TModel>(int id, QueryModel queryModel = null, ContextModel contextModel = null, string action = null, IDictionary < string, object> extraParameters = null) where TModel : BaseModel 
            => _apiRepositoryAsync.GetObjectAsync<TModel>(id, queryModel, contextModel, action, extraParameters).Result;
        

        /// <summary>
        /// Get an object by identifier, seralized into TModel. 
        /// </summary>
        public TModel GetObject<TModel>(string identifier, QueryModel queryModel = null, ContextModel contextModel = null, string action = null, IDictionary <string, object> extraParameters = null) where TModel : BaseModel
            => _apiRepositoryAsync.GetObjectAsync<TModel>(identifier, queryModel, contextModel, action, extraParameters).Result;
        

        /// <summary>
        /// Get many objects, serialized into TModel. Query parameters, outputheaders, context/culture and which languages to query for are all optional.
        /// </summary>
        public IEnumerable<TModel> GetMany<TModel>(QueryModel queryModel = null, ApiResponseHeadersModel headers = null,ContextModel contextModel = null, 
            string action = null, List<string> languages = null, IDictionary<string, object> extraParameters = null)
            => _apiRepositoryAsync.GetManyAsync<TModel>(queryModel, headers, contextModel, action, languages, extraParameters).Result;
        

        /// <summary>
        /// Get many objects, untyped. Query parameters, outputheaders, context/culture and which languages to query for are all optional.
        /// </summary>
        public IEnumerable<object> GetMany(Type modelType, QueryModel queryModel = null, ApiResponseHeadersModel headers = null, ContextModel contextModel = null, string action = null, List<string> languages = null, IDictionary<string, object> extraParameters = null)
            => _apiRepositoryAsync.GetManyAsync(modelType, queryModel, headers, contextModel, action, languages, extraParameters).Result;
        /// <summary>
        /// Get the next page of items from a previous request.
        /// </summary>
        public IEnumerable<TModel> GetNextPage<TModel>(ApiResponseHeadersModel headersOfPreviousRequest)
            => _apiRepositoryAsync.GetNextPageAsync<TModel>(headersOfPreviousRequest).Result;

        /// <summary>
        /// Get the previous page of items from a previous request.
        /// </summary>
        public IEnumerable<TModel> GetPreviousPage<TModel>(ApiResponseHeadersModel headersOfPreviousRequest)
            => _apiRepositoryAsync.GetPreviousPageAsync<TModel>(headersOfPreviousRequest).Result;


        /// <summary>
        /// Deletes an object by id.
        /// </summary>
        public bool DeleteObject<TModel>(int id) where TModel : BaseModel => _apiRepositoryAsync.DeleteObjectAsync<TModel>(id).Result;

        /// <summary>
        /// Deletes an object by identifier.
        /// </summary>
        public bool DeleteObject<TModel>(string identifier) where TModel : BaseModel => _apiRepositoryAsync.DeleteObjectAsync<TModel>(identifier).Result;
        
        /// <summary>
        /// Saves an object. Specify specific action in url, responsetype to serialize the response into, and context/culture, as needed.
        /// </summary>
        public object SaveObject<TModel>(JObject jsonItem, string action = null, Type responseType = null, ContextModel contextModel = null, bool verifyIdentifierNotTaken = true, IDictionary<string, object> extraParameters = null) where TModel : BaseModel
            => _apiRepositoryAsync.SaveObjectAsync<TModel>(jsonItem, action, responseType, contextModel, verifyIdentifierNotTaken, extraParameters).Result;
    }
}
