using System;
using System.Collections.Generic;
using System.Net.Http;
using Wipcore.Enova.Api.Abstractions.Models;

namespace Wipcore.Enova.Api.Abstractions.Interfaces
{
    /// <summary>
    /// Client for sending requests to an API.
    /// </summary>
    public interface IApiClient
    {
        /// <summary>
        /// Access the internal http client used to communicate with the API. 
        /// </summary>
        HttpClient InternalHttpClient { get; set; }

        /// <summary>
        /// Check if one object exists.
        /// </summary>
        /// <param name="controller">The controller path of the the request, i.e. "products" in /api/products/</param>
        /// <param name="identifier">Identifier of the object to check.</param>
        bool ObjectExists(string controller, string identifier);

        /// <summary>
        /// Check if one object exists.
        /// </summary>
        /// <param name="controller">The controller path of the the request, i.e. "products" in /api/products/</param>
        /// <param name="id">ID of the object to check.</param>
        bool ObjectExists(string controller, int id);

        /// <summary>
        /// Get one object, untyped.
        /// </summary>
        /// <param name="resonseType">Type to serialize the object into.</param>
        /// <param name="controller">The controller path of the the request, i.e. "product" in /api/product/</param>
        /// <param name="id">ID of the object to get.</param>
        /// <param name="action">Action part of the url if there is one</param>
        /// <param name="queryModel">Paging, size, sort, filter etc.</param>
        /// <param name="contextModel">Context/culture values such as language.</param>
        /// <param name="extraParameters">Any extra query parameters.</param>
        /// <param name="headers">Recived headers if object is given.</param>
        /// <param name="throwIfNotFound">Set to true to throw exception if object does not exist.</param>
        object GetOne(Type resonseType, string controller, int id, string action = null, QueryModel queryModel = null, IContextModel contextModel = null,
            IDictionary<string, object> extraParameters = null, ApiResponseHeadersModel headers = null, bool throwIfNotFound = false);

        /// <summary>
        /// Get one object, serialized into TModel.
        /// </summary>
        /// <param name="resonseType">Type to serialize the object into.</param>
        /// <param name="controller">The controller path of the the request, i.e. "product" in /api/product/</param>
        /// <param name="identifier">Identifier of the object to get.</param>
        /// <param name="action">Action part of the url if there is one</param>
        /// <param name="queryModel">Paging, size, sort, filter etc.</param>
        /// <param name="contextModel">Context/culture values such as language.</param>
        /// <param name="extraParameters">Any extra query parameters.</param>
        /// <param name="headers">Recived headers if object is given.</param>
        /// <param name="throwIfNotFound">Set to true to throw exception if object does not exist.</param>
        object GetOne(Type resonseType, string controller, string identifier = null, string action = null, QueryModel queryModel = null, IContextModel contextModel = null,
           IDictionary<string, object> extraParameters = null, ApiResponseHeadersModel headers = null, bool throwIfNotFound = false);

        /// <summary>
        /// Get one object, untyped.
        /// </summary>
        /// <param name="controller">The controller path of the the request, i.e. "product" in /api/product/</param>
        /// <param name="identifier">Identifier of the object to get.</param>
        /// <param name="action">Action part of the url if there is one</param>
        /// <param name="queryModel">Paging, size, sort, filter etc.</param>
        /// <param name="contextModel">Context/culture values such as language.</param>
        /// <param name="extraParameters">Any extra query parameters.</param>
        /// <param name="headers">Recived headers if object is given.</param>
        /// <param name="throwIfNotFound">Set to true to throw exception if object does not exist.</param>
        TModel GetOne<TModel>(string controller, string identifier = null, string action = null, QueryModel queryModel = null, IContextModel contextModel = null,
            IDictionary<string, object> extraParameters = null, ApiResponseHeadersModel headers = null, bool throwIfNotFound = false) where TModel : class;

        /// <summary>
        /// Get one object,  serialized into TModel.
        /// </summary>
        /// <param name="controller">The controller path of the the request, i.e. "product" in /api/product/</param>
        /// <param name="id">ID of the object to get.</param>
        /// <param name="action">Action part of the url if there is one</param>
        /// <param name="queryModel">Paging, size, sort, filter etc.</param>
        /// <param name="contextModel">Context/culture values such as language.</param>
        /// <param name="extraParameters">Any extra query parameters.</param>
        /// <param name="headers">Recived headers if object is given.</param>
        /// <param name="throwIfNotFound">Set to true to throw exception if object does not exist.</param>
        TModel GetOne<TModel>(string controller, int id, string action = null, QueryModel queryModel = null, IContextModel contextModel = null,
            IDictionary<string, object> extraParameters = null, ApiResponseHeadersModel headers = null, bool throwIfNotFound = false) where TModel : class;

        /// <summary>
        /// Get many objects from the api, serialized to model type.
        /// </summary>
        /// <typeparam name="TModel">The type to serialize the items into.</typeparam>
        /// <param name="controller">The controller path of the the request, i.e. "product" in /api/product/</param>
        /// <param name="queryModel">Paging, size, sort, filter etc.</param>
        /// <param name="contextModel">Context/culture values such as language.</param>
        /// <param name="action">Action part of the url if there is one.</param>
        /// <param name="extraParameters">Any extra query parameters.</param>
        /// <param name="headers">Recived headers if object is given.</param>
        IEnumerable<TModel> GetMany<TModel>(string controller, IQueryModel queryModel = null, IContextModel contextModel = null, string action = null,
            IDictionary<string, object> extraParameters = null, ApiResponseHeadersModel headers = null);

        /// <summary>
        /// Get many objects from the api, untyped.
        /// </summary>
        /// <param name="controller">The controller path of the the request, i.e. "product" in /api/product/</param>
        /// <param name="queryModel">Paging, size, sort, filter etc.</param>
        /// <param name="contextModel">Context/culture values such as language.</param>
        /// <param name="action">Action part of the url if there is one.</param>
        /// <param name="extraParameters">Any extra query parameters.</param>
        /// <param name="headers">Recived headers if object is given.</param>
        IEnumerable<object> GetMany(string controller, IQueryModel queryModel = null, IContextModel contextModel = null, string action = null,
            IDictionary<string, object> extraParameters = null, ApiResponseHeadersModel headers = null);

        /// <summary>
        /// Get the next page of items from a previous request.
        /// </summary>
        /// <typeparam name="TModel">The type to serialize the items into.</typeparam>
        /// <param name="headersOfPreviousRequest">Headers received from previous request.</param>
        IEnumerable<TModel> GetNextPage<TModel>(ApiResponseHeadersModel headersOfPreviousRequest);

        /// <summary>
        /// Get the previous page of items from a previous request.
        /// </summary>
        /// <typeparam name="TModel">The type to serialize the items into.</typeparam>
        /// <param name="headersOfPreviousRequest">Headers received from previous request.</param>
        IEnumerable<TModel> GetPreviousPage<TModel>(ApiResponseHeadersModel headersOfPreviousRequest);
        

        /// <summary>
        /// Delete one object.
        /// </summary>
        /// <param name="controller">The controller path of the the request, i.e. "product" in /api/product/</param>
        /// <param name="id">ID of the object to delete.</param>
        /// <param name="throwIfNotFound">Set to true to throw exception if object does not exist.</param>
        bool DeleteOne(string controller, int id, bool throwIfNotFound = false);

        /// <summary>
        /// Delete one object.
        /// </summary>
        /// <param name="controller">The controller path of the the request, i.e. "product" in /api/product/</param>
        /// <param name="identifier">Identifier of the object to delete.</param>
        /// <param name="throwIfNotFound">Set to true to throw exception if object does not exist.</param>
        bool DeleteOne(string controller, string identifier, bool throwIfNotFound = false);

        /// <summary>
        /// Saves one item.
        /// </summary>
        /// <param name="controller">The controller path of the the request, i.e. "product" in /api/product/</param>
        /// <param name="action">Action part of the url if there is one</param>
        /// <param name="contextModel">Context/culture values such as language.</param>
        /// <param name="extraParameters">Any extra query parameters.</param>
        /// <param name="item">String serlialized json item.</param>
        /// <param name="responseType">The type to serialize the response into.</param>
        /// <param name="put">True for a put request, false for a post.</param>
        object SaveOne(string controller, string item, Type responseType, string action = null,IContextModel contextModel = null, IDictionary<string, object> extraParameters = null, bool put = true);

        /// <summary>
        /// Returns true if enova is responding through the API.
        /// </summary>
        bool IsEnovaAlive();

        /// <summary>
        /// Returns true if the current user is logged in. Depends on having a token cookie.
        /// </summary>
        bool IsLoggedIn();

        /// <summary>
        /// Get information about the currently logged in user.
        /// </summary>
        IDictionary<string, string> GetLoggedInUserInfo();

        /// <summary>
        /// Get information about the Enova node behind the API.
        /// </summary>
        IDictionary<string, string> GetEnovaNodeInfo();

        /// <summary>
        /// Login an admin
        /// </summary>
        ILoginResponseModel LoginAdmin(string alias, string password);

        /// <summary>
        /// Login a customer
        /// </summary>
        ILoginResponseModel LoginCustomer(string alias, string password);

        /// <summary>
        /// Login a customer as admin
        /// </summary>
        ILoginResponseModel LoginCustomerAsAdmin(string customerAlias, string adminAlias, string adminPassword);

        /// <summary>
        /// Logout the user.
        /// </summary>
        bool Logout();
    }
}