using System;
using System.Collections.Generic;
using System.Net.Http;
using Wipcore.Enova.Api.Abstractions.Interfaces;
using Wipcore.Enova.Api.Abstractions.Models;

namespace Wipcore.Enova.Api.NetClient
{
    /// <summary>
    /// Client for sending requests to an API. This is a sync wrapper around ApiClientAsync.
    /// </summary>
    public class ApiClient : IApiClient
    {
        private readonly IApiClientAsync _apiClientAsync;

        public ApiClient(IApiClientAsync apiClientAsync)
        {
            _apiClientAsync = apiClientAsync;
        }

        /// <summary>
        /// Access the internal http client used to communicate with the API. 
        /// </summary>
        public HttpClient InternalHttpClient {
            get => _apiClientAsync.InternalHttpClient;
            set => _apiClientAsync.InternalHttpClient = value;
        }

        /// <summary>
        /// Check if one object exists.
        /// </summary>
        /// <param name="controller">The controller path of the the request, i.e. "products" in /api/products/</param>
        /// <param name="identifier">Identifier of the object to check.</param>
        public bool ObjectExists(string controller, string identifier) => _apiClientAsync.ObjectExistsAsync(controller, identifier).Result;


        /// <summary>
        /// Check if one object exists.
        /// </summary>
        /// <param name="controller">The controller path of the the request, i.e. "products" in /api/products/</param>
        /// <param name="id">ID of the object to check.</param>
        public bool ObjectExists(string controller, int id) => _apiClientAsync.ObjectExistsAsync(controller, id).Result;
        
        /// <summary>
        /// Get one object, untyped.
        /// </summary>
        /// <param name="resonseType">Type to serialize the object into.</param>
        /// <param name="controller">The controller path of the the request, i.e. "products" in /api/products/</param>
        /// <param name="id">ID of the object to get.</param>
        /// <param name="action">Action part of the url if there is one</param>
        /// <param name="queryModel">Paging, size, sort, filter etc.</param>
        /// <param name="contextModel">Context/culture values such as language.</param>
        /// <param name="extraParameters">Any extra query parameters.</param>
        /// <param name="headers">Recived headers if object is given.</param>
        /// <param name="throwIfNotFound">Set to true to throw exception if object does not exist.</param>
        public object GetOne(Type resonseType, string controller, int id, string action = null, QueryModel queryModel = null, IContextModel contextModel = null,
            IDictionary<string, object> extraParameters = null, ApiResponseHeadersModel headers = null, bool throwIfNotFound = false)
        {
            return _apiClientAsync.GetOneAsync(resonseType, controller, "id-" + id, action, queryModel, contextModel, extraParameters, headers, throwIfNotFound).Result;
        }

        /// <summary>
        /// Get one object, serialized into TModel.
        /// </summary>
        /// <param name="controller">The controller path of the the request, i.e. "products" in /api/products/</param>
        /// <param name="identifier">Identifier of the object to get.</param>
        /// <param name="action">Action part of the url if there is one</param>
        /// <param name="queryModel">Paging, size, sort, filter etc.</param>
        /// <param name="contextModel">Context/culture values such as language.</param>
        /// <param name="extraParameters">Any extra query parameters.</param>
        /// <param name="headers">Recived headers if object is given.</param>
        /// <param name="throwIfNotFound">Set to true to throw exception if object does not exist.</param>
        public TModel GetOne<TModel>(string controller, string identifier = null, string action = null, QueryModel queryModel = null, IContextModel contextModel = null,
            IDictionary<string, object> extraParameters = null, ApiResponseHeadersModel headers = null, bool throwIfNotFound = false) where TModel : class
        {
            return (TModel) _apiClientAsync.GetOneAsync(typeof(TModel), controller, identifier, action, queryModel, contextModel, extraParameters, headers, throwIfNotFound).Result;
        }

        /// <summary>
        /// Get one object,  serialized into TModel.
        /// </summary>
        /// <param name="controller">The controller path of the the request, i.e. "products" in /api/products/</param>
        /// <param name="id">ID of the object to get.</param>
        /// <param name="action">Action part of the url if there is one</param>
        /// <param name="queryModel">Paging, size, sort, filter etc.</param>
        /// <param name="contextModel">Context/culture values such as language.</param>
        /// <param name="extraParameters">Any extra query parameters.</param>
        /// <param name="headers">Recived headers if object is given.</param>
        /// <param name="throwIfNotFound">Set to true to throw exception if object does not exist.</param>
        public TModel GetOne<TModel>(string controller, int id, string action = null, QueryModel queryModel = null, IContextModel contextModel = null,
            IDictionary<string, object> extraParameters = null, ApiResponseHeadersModel headers = null, bool throwIfNotFound = false) where TModel : class
        {
            return (TModel)_apiClientAsync.GetOneAsync(typeof(TModel), controller, "id-" + id, action, queryModel, contextModel, extraParameters, headers, throwIfNotFound).Result;
        }

        /// <summary>
        /// Get one object, untyped.
        /// </summary>
        /// <param name="resonseType">Type to serialize the object into.</param>
        /// <param name="controller">The controller path of the the request, i.e. "products" in /api/products/</param>
        /// <param name="identifier">Identifier of the object to get.</param>
        /// <param name="action">Action part of the url if there is one</param>
        /// <param name="queryModel">Paging, size, sort, filter etc.</param>
        /// <param name="contextModel">Context/culture values such as language.</param>
        /// <param name="extraParameters">Any extra query parameters.</param>
        /// <param name="headers">Recived headers if object is given.</param>
        /// <param name="throwIfNotFound">Set to true to throw exception if object does not exist.</param>
        public object GetOne(Type resonseType, string controller, string identifier = null, string action = null, QueryModel queryModel = null, IContextModel contextModel = null,
           IDictionary<string, object> extraParameters = null, ApiResponseHeadersModel headers = null, bool throwIfNotFound = false)
        {
            return _apiClientAsync.GetOneAsync(resonseType, controller, identifier, action, queryModel, contextModel, extraParameters, headers, throwIfNotFound).Result;
        }
        
        /// <summary>
        /// Get many objects from the api, untyped.
        /// </summary>
        /// <param name="controller">The controller path of the the request, i.e. "products" in /api/products/</param>
        /// <param name="queryModel">Paging, size, sort, filter etc.</param>
        /// <param name="contextModel">Context/culture values such as language.</param>
        /// <param name="action">Action part of the url if there is one.</param>
        /// <param name="extraParameters">Any extra query parameters.</param>
        /// <param name="headers">Recived headers if object is given.</param>
        public IEnumerable<object> GetMany(string controller, IQueryModel queryModel = null, IContextModel contextModel = null, string action = null,
            IDictionary<string, object> extraParameters = null, ApiResponseHeadersModel headers = null)
        {
            return _apiClientAsync.GetManyAsync<object>(controller, queryModel, contextModel, action, extraParameters, headers).Result;
        }

        /// <summary>
        /// Get many objects from the api, serialized to model type.
        /// </summary>
        /// <typeparam name="TModel">The type to serialize the items into.</typeparam>
        /// <param name="controller">The controller path of the the request, i.e. "products" in /api/products/</param>
        /// <param name="queryModel">Paging, size, sort, filter etc.</param>
        /// <param name="contextModel">Context/culture values such as language.</param>
        /// <param name="action">Action part of the url if there is one.</param>
        /// <param name="extraParameters">Any extra query parameters.</param>
        /// <param name="headers">Recived headers if object is given.</param>
        public IEnumerable<TModel> GetMany<TModel>(string controller, IQueryModel queryModel = null, IContextModel contextModel = null, string action = null,
            IDictionary<string, object> extraParameters = null, ApiResponseHeadersModel headers = null)
        {
            return _apiClientAsync.GetManyAsync<TModel>(controller, queryModel, contextModel, action, extraParameters, headers).Result;
        }

        /// <summary>
        /// Get the next page of items from a previous request.
        /// </summary>
        /// <typeparam name="TModel">The type to serialize the items into.</typeparam>
        /// <param name="headersOfPreviousRequest">Headers received from previous request.</param>
        public IEnumerable<TModel> GetNextPage<TModel>(ApiResponseHeadersModel headersOfPreviousRequest)
        {
            return _apiClientAsync.GetNextPageAsync<TModel>(headersOfPreviousRequest).Result;
        }

        /// <summary>
        /// Get the previous page of items from a previous request.
        /// </summary>
        /// <typeparam name="TModel">The type to serialize the items into.</typeparam>
        /// <param name="headersOfPreviousRequest">Headers received from previous request.</param>
        public IEnumerable<TModel> GetPreviousPage<TModel>(ApiResponseHeadersModel headersOfPreviousRequest)
        {
            return _apiClientAsync.GetPreviousPageAsync<TModel>(headersOfPreviousRequest).Result;
        }

        /// <summary>
        /// Delete one object. Returns true if deleted, false if not found (unless set to throw exception when not found). 
        /// </summary>
        /// <param name="controller">The controller path of the the request, i.e. "products" in /api/products/</param>
        /// <param name="id">ID of the object to delete.</param>
        /// <param name="throwIfNotFound">Set to true to throw exception if object does not exist.</param>
        public bool DeleteOne(string controller, int id, bool throwIfNotFound = false)
        {
            return _apiClientAsync.DeleteOneAsync(controller, id, throwIfNotFound).Result;
        }

        /// <summary>
        /// Delete one object. Returns true if deleted, false if not found (unless set to throw exception when not found). 
        /// </summary>
        /// <param name="controller">The controller path of the the request, i.e. "products" in /api/products/</param>
        /// <param name="identifier">Identifier of the object to delete.</param>
        /// <param name="throwIfNotFound">Set to true to throw exception if object does not exist.</param>
        public bool DeleteOne(string controller, string identifier, bool throwIfNotFound = false)
        {
            return _apiClientAsync.DeleteOneAsync(controller, identifier, throwIfNotFound).Result;
        }

        /// <summary>
        /// Saves one item.
        /// </summary>
        /// <param name="controller">The controller path of the the request, i.e. "products" in /api/products/</param>
        /// <param name="action">Action part of the url if there is one</param>
        /// <param name="contextModel">Context/culture values such as language.</param>
        /// <param name="extraParameters">Any extra query parameters.</param>
        /// <param name="item">String serlialized json item.</param>
        /// <param name="responseType">The type to serialize the response into.</param>
        /// <param name="put">True for a put request, false for a post.</param>
        /// <returns></returns>
        public object SaveOne(string controller, string item, Type responseType, string action = null, IContextModel contextModel = null, IDictionary<string, object> extraParameters = null, bool put = true)
        {
            return _apiClientAsync.SaveOneAsync(controller, item, responseType, action, contextModel, extraParameters, put).Result;
        }

        /// <summary>
        /// Returns true if enova is responding through the API.
        /// </summary>
        public bool IsEnovaAlive()
        {
            return _apiClientAsync.IsEnovaAliveAsync().Result;
        }

        /// <summary>
        /// Returns true if the current user is logged in. Depends on having a token cookie.
        /// </summary>
        public bool IsLoggedIn()
        {
            return _apiClientAsync.IsLoggedInAsync().Result;
        }

        /// <summary>
        /// Get information about the currently logged in user.
        /// </summary>
        public IDictionary<string, string> GetLoggedInUserInfo()
        {
            return _apiClientAsync.GetLoggedInUserInfoAsync().Result;
        }

        /// <summary>
        /// Get information about the Enova node behind the API.
        /// </summary>
        public IDictionary<string, string> GetEnovaNodeInfo()
        {
            return _apiClientAsync.GetEnovaNodeInfoAsync().Result;
        }

        /// <summary>
        /// Logout the user.
        /// </summary>
        public bool Logout()
        {
            return _apiClientAsync.LogoutAsync().Result;
        }

        /// <summary>
        /// Login an admin
        /// </summary>
        public ILoginResponseModel LoginAdmin(string alias, string password)
        {
            return _apiClientAsync.LoginAdminAsync(alias, password).Result;
        }

        /// <summary>
        /// Login a customer
        /// </summary>
        public ILoginResponseModel LoginCustomer(string alias, string password)
        {
            return _apiClientAsync.LoginCustomerAsync(alias, password).Result;
        }

        /// <summary>
        /// Login a customer as admin
        /// </summary>
        public ILoginResponseModel LoginCustomerAsAdmin(string customerAlias, string adminAlias, string adminPassword)
        {
            return _apiClientAsync.LoginCustomerAsAdminAsync(customerAlias, adminAlias, adminPassword).Result;
        }
    }
}
