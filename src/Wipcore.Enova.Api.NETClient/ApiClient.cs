using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Wipcore.Enova.Api.Abstractions.Interfaces;
using Wipcore.Enova.Api.Abstractions.Models;
using System.Web.Http;
using Wipcore.Enova.Api.Abstractions;

namespace Wipcore.eNova.Api.NETClient
{
    /// <summary>
    /// Client for sending requests to an API.
    /// </summary>
    public class ApiClient : IApiClient
    {
        private const string TokenKey = "ApiToken";
        private readonly IConfigurationRoot _root;
        private readonly IHttpContextAccessor _httpAccessor;
        private readonly HttpClient _client;
        private readonly IDataProtector _protector;
        private readonly ILogger _log;

        public ApiClient(IConfigurationRoot root, IHttpContextAccessor httpAccessor, IDataProtectionProvider protectionProvider, ILoggerFactory loggerFactory)
        {
            _root = root;
            _httpAccessor = httpAccessor;
            _protector = protectionProvider.CreateProtector("CookieEncrypter");
            _client = new HttpClient { BaseAddress = new Uri(root["API:Url"] ?? "http://localhost:5000/api/") };
            _log = loggerFactory.CreateLogger(GetType().Namespace);

            //if the end user has a token cookie, then place the token in the header for requests made by this client
            var tokenCookie = _httpAccessor.HttpContext?.Request.Cookies[TokenKey];
            if (!String.IsNullOrEmpty(tokenCookie))
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + tokenCookie);
        }

        /// <summary>
        /// Check if one object exists.
        /// </summary>
        /// <param name="controller">The controller path of the the request, i.e. "products" in /api/products/</param>
        /// <param name="identifier">Identifier of the object to check.</param>
        public bool ObjectExists(string controller, string identifier)
        {
            identifier = identifier ?? String.Empty;
            var url = $"{controller}/{identifier}";
            var response = _client.SendAsync(new HttpRequestMessage(HttpMethod.Head, url)).Result;
            var responseContent = response.Content.ReadAsStringAsync().Result;

            if (response.IsSuccessStatusCode)
                return true;
            if (response.StatusCode == HttpStatusCode.NotFound)
                return false;
            
            throw new HttpResponseException(new HttpResponseMessage()
            {
                StatusCode = response.StatusCode,
                ReasonPhrase = $"Head url {_client.BaseAddress}{url} gave error: {response.ReasonPhrase}. Details: {responseContent}"
            });
        }

        /// <summary>
        /// Check if one object exists.
        /// </summary>
        /// <param name="controller">The controller path of the the request, i.e. "products" in /api/products/</param>
        /// <param name="id">ID of the object to check.</param>
        public bool ObjectExists(string controller, int id)
        {
            return ObjectExists(controller, "id-" + id);
        }

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
        public object GetOne(Type resonseType, string controller, int id, string action = null, QueryModel queryModel = null, IContextModel contextModel = null,
            IDictionary<string, object> extraParameters = null, ApiResponseHeadersModel headers = null)
        {
            return GetOne(resonseType, controller, "id-" + id, action, queryModel, contextModel, extraParameters, headers);
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
        public TModel GetOne<TModel>(string controller, string identifier = null, string action = null, QueryModel queryModel = null, IContextModel contextModel = null,
            IDictionary<string, object> extraParameters = null, ApiResponseHeadersModel headers = null) where TModel : class
        {
            return (TModel)GetOne(typeof(TModel), controller, identifier, action, queryModel, contextModel, extraParameters, headers);
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
        public TModel GetOne<TModel>(string controller, int id, string action = null, QueryModel queryModel = null, IContextModel contextModel = null,
            IDictionary<string, object> extraParameters = null, ApiResponseHeadersModel headers = null) where TModel : class
        {

            return (TModel)GetOne(typeof(TModel), controller, "id-" + id, action, queryModel, contextModel, extraParameters, headers);
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
        public object GetOne(Type resonseType, string controller, string identifier = null, string action = null, QueryModel queryModel = null, IContextModel contextModel = null,
           IDictionary<string, object> extraParameters = null, ApiResponseHeadersModel headers = null)
        {
            action = action == null ? string.Empty : "/" + action;
            identifier = identifier ?? String.Empty;
            var url = $"{controller}/{identifier}{action}{BuildParameters(contextModel, queryModel, extraParameters)}";
            var response = _client.GetAsync(url).Result;
            var responseContent = response.Content.ReadAsStringAsync().Result;

            if (!response.IsSuccessStatusCode)
                throw new HttpResponseException(new HttpResponseMessage()
                {
                    StatusCode = response.StatusCode,
                    ReasonPhrase = $"Get url {_client.BaseAddress}{url}  gave error: {response.ReasonPhrase}. Details: {responseContent}"
                });

            SetResponseHeaders(response, headers);
            var model = JsonConvert.DeserializeObject(responseContent, resonseType);
            return model;
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
        /// <returns></returns>
        public IEnumerable<TModel> GetMany<TModel>(string controller, IQueryModel queryModel = null, IContextModel contextModel = null, string action = null,
            IDictionary<string, object> extraParameters = null, ApiResponseHeadersModel headers = null)
        {
            return GetMany(controller, queryModel, contextModel, action, extraParameters, headers).Cast<TModel>();
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
            action = action == null ? string.Empty : "/" + action;
            var url = $"{controller}{action}{BuildParameters(contextModel, queryModel, extraParameters)}";
            var response = _client.GetAsync(url).Result;
            var responseContent = response.Content.ReadAsStringAsync().Result;

            if (!response.IsSuccessStatusCode)
                throw new HttpResponseException(new HttpResponseMessage()
                {
                    StatusCode = response.StatusCode,
                    ReasonPhrase = $"Get url {_client.BaseAddress}{url}  gave error: {response.ReasonPhrase}. Details: {responseContent}"
                });

            SetResponseHeaders(response, headers);
            var models = JsonConvert.DeserializeObject<IEnumerable<object>>(responseContent);

            return models;
        }

        /// <summary>
        /// Delete one object.
        /// </summary>
        /// <param name="controller">The controller path of the the request, i.e. "products" in /api/products/</param>
        /// <param name="id">ID of the object to delete.</param>
        /// <returns></returns>
        public bool DeleteOne(string controller, int id)
        {
            var url = $"{controller}/id-{id}";
            var response = _client.DeleteAsync(url).Result;

            if (response.IsSuccessStatusCode)
                return true;

            var responseContent = response.Content.ReadAsStringAsync().Result;
            throw new HttpResponseException(new HttpResponseMessage()
            {
                StatusCode = response.StatusCode,
                ReasonPhrase = $"Delete url {_client.BaseAddress}{url} gave error: {response.ReasonPhrase}. Details: {responseContent}"
            });
        }

        /// <summary>
        /// Delete one object.
        /// </summary>
        /// <param name="controller">The controller path of the the request, i.e. "products" in /api/products/</param>
        /// <param name="identifier">Identifier of the object to delete.</param>
        public bool DeleteOne(string controller, string identifier)
        {
            var url = $"{controller}/{identifier}";
            var response = _client.DeleteAsync(url).Result;

            if (response.IsSuccessStatusCode)
                return true;

            var responseContent = response.Content.ReadAsStringAsync().Result;
            throw new HttpResponseException(new HttpResponseMessage()
            {
                StatusCode = response.StatusCode,
                ReasonPhrase = $"Delete url {_client.BaseAddress}{url} gave error: {response.ReasonPhrase}. Details: {responseContent}"
            });
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
            action = action == null ? string.Empty : action + "/";
            var url = $"{controller}/{action}{BuildParameters(contextModel, null, extraParameters)}";
            var stringContent = new StringContent(item, Encoding.UTF8, "application/json");
            var response = put ? _client.PutAsync(url, stringContent).Result : _client.PostAsync(url, stringContent).Result;
            var responseContent = response.Content.ReadAsStringAsync().Result;

            if (!response.IsSuccessStatusCode)
                throw new HttpResponseException(new HttpResponseMessage()
                {
                    StatusCode = response.StatusCode,
                    ReasonPhrase = $"Save url {_client.BaseAddress}{url}  gave error: {response.ReasonPhrase}. Details: {responseContent}"
                });

            var result = response.Content.ReadAsStringAsync().Result;
            var model = JsonConvert.DeserializeObject(result, responseType);
            return model;
        }


        /// <summary>
        /// Returns true if enova is responding through the API.
        /// </summary>
        public bool IsEnovaAlive()
        {
            try
            {
                var response = _client.GetAsync("/IsEnovaAlive").Result;
                return Convert.ToBoolean(response.Content.ReadAsStringAsync().Result);
            }
            catch (Exception e)
            {
                _log.LogDebug("Enova did not respond it was alive: {0}", e);
                return false;
            }
        }

        /// <summary>
        /// Returns true if the current user is logged in. Depends on having a token cookie.
        /// </summary>
        public bool IsLoggedIn()
        {
            var dictionary = GetLoggedInUserInfo();
            return Convert.ToBoolean(dictionary["LoggedIn"]);
        }

        /// <summary>
        /// Get information about the currently logged in user.
        /// </summary>
        public IDictionary<string, string> GetLoggedInUserInfo()
        {
            var response = _client.GetAsync("/Account/LoggedInAs").Result;
            var responseContent = response.Content.ReadAsStringAsync().Result;

            if (!response.IsSuccessStatusCode)
                throw new HttpResponseException(new HttpResponseMessage()
                {
                    StatusCode = response.StatusCode,
                    ReasonPhrase = $"Get url {_client.BaseAddress}/Account/LoggedInAs gave error: {response.ReasonPhrase}. Details: {responseContent}"
                });

            var dictionary = JsonConvert.DeserializeObject<IDictionary<string, string>>(responseContent);
            return dictionary;
        }

        /// <summary>
        /// Get information about the Enova node behind the API.
        /// </summary>
        public IDictionary<string, string> GetEnovaNodeInfo()
        {
            var response = _client.GetAsync("/NodeInfo").Result;
            var responseContent = response.Content.ReadAsStringAsync().Result;

            if (!response.IsSuccessStatusCode)
                throw new HttpResponseException(new HttpResponseMessage()
                {
                    StatusCode = response.StatusCode,
                    ReasonPhrase = $"Get url {_client.BaseAddress}/NodeInfo gave error: {response.ReasonPhrase}. Details: {responseContent}"
                });

            var dictionary = JsonConvert.DeserializeObject<IDictionary<string, string>>(responseContent);
            return dictionary;
        }

        /// <summary>
        /// Logout the user.
        /// </summary>
        public bool Logout()
        {
            var response = _client.PostAsync("/Account/Logout", null).Result;
            var responseContent = response.Content.ReadAsStringAsync().Result;
            if (!response.IsSuccessStatusCode)
                throw new HttpResponseException(new HttpResponseMessage()
                {
                    StatusCode = response.StatusCode,
                    ReasonPhrase = $"Post url {_client.BaseAddress}/Account/Logout gave error: {response.ReasonPhrase}. Details: {responseContent}"
                });

            //clean up cookies
            var tokenCookie = _httpAccessor.HttpContext?.Request.Cookies[TokenKey];
            if (tokenCookie != null)
                _httpAccessor.HttpContext.Response.Cookies.Delete(TokenKey);
            var userIdCookie = _httpAccessor.HttpContext?.Request.Cookies[WipConstants.UserIdCookieIdentifier];
            if (userIdCookie != null)
                _httpAccessor.HttpContext.Response.Cookies.Delete(userIdCookie);

            return true;
        }

        /// <summary>
        /// Login an admin
        /// </summary>
        public ILoginResponseModel LoginAdmin(string alias, string password)
        {
            return Login(new LoginModel() { Alias = alias, Password = password }, "/Account/LoginAdmin");
        }

        /// <summary>
        /// Login a customer
        /// </summary>
        public ILoginResponseModel LoginCustomer(string alias, string password)
        {
            return Login(new LoginModel() { Alias = alias, Password = password }, "/Account/LoginCustomer");
        }

        private ILoginResponseModel Login(LoginModel model, string url)
        {
            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, new UTF8Encoding(), "application/json");

            var response = _client.PostAsync(url, content).Result;
            var responseContent = response.Content.ReadAsStringAsync().Result;

            if (!response.IsSuccessStatusCode)
                throw new HttpResponseException(new HttpResponseMessage() { StatusCode = response.StatusCode, ReasonPhrase = $"Login gave error {response.ReasonPhrase} for user {model?.Alias ?? "null"} . Details: {responseContent}" });

            var loginModel = JsonConvert.DeserializeObject<LoginResponseModel>(responseContent);

            var httpContext = _httpAccessor.HttpContext;
            if (!String.IsNullOrEmpty(loginModel.AccessToken))
            {
                //if elastic thread, then this context is internal within the app, thus set directly on next request. Otherwise send to client as a response
                if (httpContext.TraceIdentifier == WipConstants.ElasticIndexHttpContextIdentifier || httpContext.TraceIdentifier == WipConstants.ElasticDeltaIndexHttpContextIdentifier)
                    httpContext.Request.Cookies = new RequestCookieCollection(new Dictionary<string, string>() { { TokenKey, loginModel.AccessToken } });
                else
                    httpContext.Response.Cookies.Append(TokenKey, loginModel.AccessToken);
            }

            if (loginModel.UserId != null)
            {
                //add the id of the user in another cookie. Useful for getting the current users data (like filters) without querying api every time.
                var protectedId = _protector.Protect(loginModel.UserId.Value.ToString());
                httpContext.Response.Cookies.Append(WipConstants.UserIdCookieIdentifier, protectedId);
            }

            return loginModel;
        }

        private void SetResponseHeaders(HttpResponseMessage response, ApiResponseHeadersModel headers)
        {
            if (headers == null)
                return;

            IEnumerable<string> values;
            if (response.Headers.TryGetValues("X-Paging-PageNo", out values))
                headers.PageNumber = Convert.ToInt32(values.First());
            if (response.Headers.TryGetValues("X-Paging-PageSize", out values))
                headers.PageSize = Convert.ToInt32(values.First());
            if (response.Headers.TryGetValues("X-Paging-PageCount", out values))
                headers.PageCount = Convert.ToInt32(values.First());
            if (response.Headers.TryGetValues("X-Paging-TotalRecordCount", out values))
                headers.TotalRecordsCount = Convert.ToInt32(values.First());
        }

        private string BuildParameters(IContextModel context, IQueryModel queryModel, IDictionary<string, object> extraParameters)
        {
            if (context == null && queryModel == null && extraParameters == null)
                return String.Empty;

            var sb = new StringBuilder();
            sb.Append("?");
            if (context != null)
            {
                foreach (var p in typeof(IContextModel).GetProperties())
                {
                    var value = p.GetValue(context);
                    if (value == null)
                        continue;

                    sb.Append(p.Name);
                    sb.Append("=");
                    sb.Append(value);
                    sb.Append("&");
                }

                if (queryModel == null && (extraParameters == null || !extraParameters.Any()))
                    sb.Remove(sb.Length - 1, 1);//removing last &..
            }

            if (queryModel != null)
            {
                foreach (var p in typeof(IQueryModel).GetProperties())
                {
                    var value = p.GetValue(queryModel);
                    if (value == null)
                        continue;

                    sb.Append(p.Name);
                    sb.Append("=");
                    sb.Append(value);
                    sb.Append("&");
                }

                if (extraParameters == null || !extraParameters.Any())
                    sb.Remove(sb.Length - 1, 1);
            }

            if (extraParameters != null && extraParameters.Any())
            {
                foreach (var parameter in extraParameters)
                {
                    sb.Append(parameter.Key);
                    sb.Append("=");
                    sb.Append(parameter.Value);
                    sb.Append("&");
                }
                sb.Remove(sb.Length - 1, 1);
            }

            return sb.ToString();
        }
    }
}
