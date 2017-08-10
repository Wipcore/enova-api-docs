using System;
using System.Collections;
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
        private readonly IHttpContextAccessor _httpAccessor;
        
        private readonly IDataProtector _protector;
        private readonly ILogger _log;

        public ApiClient(IConfigurationRoot root, IHttpContextAccessor httpAccessor, IDataProtectionProvider protectionProvider, ILoggerFactory loggerFactory)
        {
            _httpAccessor = httpAccessor;
            _protector = protectionProvider.CreateProtector("CookieEncrypter");
            InternalHttpClient = new HttpClient { BaseAddress = new Uri(root["API:Url"] ?? "http://localhost:5000/api/") };
            _log = loggerFactory.CreateLogger(GetType().Namespace);

            //if the end user has a token cookie, then place the token in the header for requests made by this client
            var tokenCookie = _httpAccessor.HttpContext?.Request.Cookies[TokenKey];
            if (!String.IsNullOrEmpty(tokenCookie))
                InternalHttpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + tokenCookie);
        }

        /// <summary>
        /// Access the internal http client used to communicate with the API. 
        /// </summary>
        public HttpClient InternalHttpClient { get; set; }

        /// <summary>
        /// Check if one object exists.
        /// </summary>
        /// <param name="controller">The controller path of the the request, i.e. "products" in /api/products/</param>
        /// <param name="identifier">Identifier of the object to check.</param>
        public bool ObjectExists(string controller, string identifier)
        {
            identifier = identifier ?? String.Empty;
            var url = $"{controller}/{identifier}";
            var response = InternalHttpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, url)).Result;
            var responseContent = response.Content.ReadAsStringAsync().Result;

            if (response.IsSuccessStatusCode)
                return true;
            if (response.StatusCode == HttpStatusCode.NotFound)
                return false;
            
            throw new HttpResponseException(new HttpResponseMessage()
            {
                StatusCode = response.StatusCode,
                ReasonPhrase = $"Head url {InternalHttpClient.BaseAddress}{url} gave error: {response.ReasonPhrase}. Details: {responseContent}"
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
        /// <param name="throwIfNotFound">Set to true to throw exception if object does not exist.</param>
        public object GetOne(Type resonseType, string controller, int id, string action = null, QueryModel queryModel = null, IContextModel contextModel = null,
            IDictionary<string, object> extraParameters = null, ApiResponseHeadersModel headers = null, bool throwIfNotFound = false)
        {
            return GetOne(resonseType, controller, "id-" + id, action, queryModel, contextModel, extraParameters, headers, throwIfNotFound);
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
            return (TModel)GetOne(typeof(TModel), controller, identifier, action, queryModel, contextModel, extraParameters, headers, throwIfNotFound);
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
            return (TModel)GetOne(typeof(TModel), controller, "id-" + id, action, queryModel, contextModel, extraParameters, headers, throwIfNotFound);
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
            action = action == null ? string.Empty : "/" + action;
            identifier = identifier ?? String.Empty;
            var url = $"{controller}/{identifier}{action}{BuildParameters(contextModel, queryModel, extraParameters)}";
            var response = InternalHttpClient.GetAsync(url).Result;
            var responseContent = response.Content.ReadAsStringAsync().Result;

            if (!throwIfNotFound && response.StatusCode == HttpStatusCode.NotFound)
                return null;

            if (!response.IsSuccessStatusCode)
                throw new HttpResponseException(new HttpResponseMessage()
                {
                    StatusCode = response.StatusCode,
                    ReasonPhrase = $"Get url {InternalHttpClient.BaseAddress}{url}  gave error: {response.ReasonPhrase}. Details: {responseContent}"
                });

            SetResponseHeaders(response, headers);
            var model = JsonConvert.DeserializeObject(responseContent, resonseType);
            return model;
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
            return GetMany<object>(controller, queryModel, contextModel, action, extraParameters, headers);
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
            action = action == null ? string.Empty : "/" + action;
            var url = $"{controller}{action}{BuildParameters(contextModel, queryModel, extraParameters)}";
            return GetManyRequest<TModel>(headers, url);
        }

        /// <summary>
        /// Get the next page of items from a previous request.
        /// </summary>
        /// <typeparam name="TModel">The type to serialize the items into.</typeparam>
        /// <param name="headersOfPreviousRequest">Headers received from previous request.</param>
        public IEnumerable<TModel> GetNextPage<TModel>(ApiResponseHeadersModel headersOfPreviousRequest)
        {
            var url = headersOfPreviousRequest.NextPageLink;
            return GetManyRequest<TModel>(headersOfPreviousRequest, url);
        }

        /// <summary>
        /// Get the previous page of items from a previous request.
        /// </summary>
        /// <typeparam name="TModel">The type to serialize the items into.</typeparam>
        /// <param name="headersOfPreviousRequest">Headers received from previous request.</param>
        public IEnumerable<TModel> GetPreviousPage<TModel>(ApiResponseHeadersModel headersOfPreviousRequest)
        {
            var url = headersOfPreviousRequest.PreviousPageLink;
            return GetManyRequest<TModel>(headersOfPreviousRequest, url);
        }

        private IEnumerable<TModel> GetManyRequest<TModel>(ApiResponseHeadersModel headers, string url)
        {
            var response = InternalHttpClient.GetAsync(url).Result;
            var responseContent = response.Content.ReadAsStringAsync().Result;

            if (!response.IsSuccessStatusCode)
                throw new HttpResponseException(new HttpResponseMessage()
                {
                    StatusCode = response.StatusCode,
                    ReasonPhrase =
                        $"Get url {InternalHttpClient.BaseAddress}{url}  gave error: {response.ReasonPhrase}. Details: {responseContent}"
                });

            SetResponseHeaders(response, headers);
            var models = JsonConvert.DeserializeObject<IEnumerable<TModel>>(responseContent);

            return models;
        }


        /// <summary>
        /// Delete one object. Returns true if deleted, false if not found (unless set to throw exception when not found). 
        /// </summary>
        /// <param name="controller">The controller path of the the request, i.e. "products" in /api/products/</param>
        /// <param name="id">ID of the object to delete.</param>
        /// <param name="throwIfNotFound">Set to true to throw exception if object does not exist.</param>
        public bool DeleteOne(string controller, int id, bool throwIfNotFound = false)
        {
            var url = $"{controller}/id-{id}";
            var response = InternalHttpClient.DeleteAsync(url).Result;

            if (response.IsSuccessStatusCode)
                return true;

            if (!throwIfNotFound && response.StatusCode == HttpStatusCode.NotFound)
                return false;

            var responseContent = response.Content.ReadAsStringAsync().Result;
            throw new HttpResponseException(new HttpResponseMessage()
            {
                StatusCode = response.StatusCode,
                ReasonPhrase = $"Delete url {InternalHttpClient.BaseAddress}{url} gave error: {response.ReasonPhrase}. Details: {responseContent}"
            });
        }

        /// <summary>
        /// Delete one object. Returns true if deleted, false if not found (unless set to throw exception when not found). 
        /// </summary>
        /// <param name="controller">The controller path of the the request, i.e. "products" in /api/products/</param>
        /// <param name="identifier">Identifier of the object to delete.</param>
        /// <param name="throwIfNotFound">Set to true to throw exception if object does not exist.</param>
        public bool DeleteOne(string controller, string identifier, bool throwIfNotFound = false)
        {
            var url = $"{controller}/{identifier}";
            var response = InternalHttpClient.DeleteAsync(url).Result;

            if (response.IsSuccessStatusCode)
                return true;

            if (!throwIfNotFound && response.StatusCode == HttpStatusCode.NotFound)
                return false;

            var responseContent = response.Content.ReadAsStringAsync().Result;
            throw new HttpResponseException(new HttpResponseMessage()
            {
                StatusCode = response.StatusCode,
                ReasonPhrase = $"Delete url {InternalHttpClient.BaseAddress}{url} gave error: {response.ReasonPhrase}. Details: {responseContent}"
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
            var response = put ? InternalHttpClient.PutAsync(url, stringContent).Result : InternalHttpClient.PostAsync(url, stringContent).Result;
            var responseContent = response.Content.ReadAsStringAsync().Result;

            if (!response.IsSuccessStatusCode)
                throw new HttpResponseException(new HttpResponseMessage()
                {
                    StatusCode = response.StatusCode,
                    ReasonPhrase = $"Save url {InternalHttpClient.BaseAddress}{url}  gave error: {response.ReasonPhrase}. Details: {responseContent}"
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
                var response = InternalHttpClient.GetAsync("/IsEnovaAlive").Result;
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
            var response = InternalHttpClient.GetAsync("/Account/LoggedInAs").Result;
            var responseContent = response.Content.ReadAsStringAsync().Result;

            if (!response.IsSuccessStatusCode)
                throw new HttpResponseException(new HttpResponseMessage()
                {
                    StatusCode = response.StatusCode,
                    ReasonPhrase = $"Get url {InternalHttpClient.BaseAddress}/Account/LoggedInAs gave error: {response.ReasonPhrase}. Details: {responseContent}"
                });

            var dictionary = JsonConvert.DeserializeObject<IDictionary<string, string>>(responseContent);
            return dictionary;
        }

        /// <summary>
        /// Get information about the Enova node behind the API.
        /// </summary>
        public IDictionary<string, string> GetEnovaNodeInfo()
        {
            var response = InternalHttpClient.GetAsync("/NodeInfo").Result;
            var responseContent = response.Content.ReadAsStringAsync().Result;

            if (!response.IsSuccessStatusCode)
                throw new HttpResponseException(new HttpResponseMessage()
                {
                    StatusCode = response.StatusCode,
                    ReasonPhrase = $"Get url {InternalHttpClient.BaseAddress}/NodeInfo gave error: {response.ReasonPhrase}. Details: {responseContent}"
                });

            var dictionary = JsonConvert.DeserializeObject<IDictionary<string, string>>(responseContent);
            return dictionary;
        }

        /// <summary>
        /// Logout the user.
        /// </summary>
        public bool Logout()
        {
            var response = InternalHttpClient.PostAsync("/Account/Logout", null).Result;
            var responseContent = response.Content.ReadAsStringAsync().Result;
            if (!response.IsSuccessStatusCode)
                throw new HttpResponseException(new HttpResponseMessage()
                {
                    StatusCode = response.StatusCode,
                    ReasonPhrase = $"Post url {InternalHttpClient.BaseAddress}/Account/Logout gave error: {response.ReasonPhrase}. Details: {responseContent}"
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

        /// <summary>
        /// Login a customer as admin
        /// </summary>
        public ILoginResponseModel LoginCustomerAsAdmin(string customerAlias, string adminAlias, string adminPassword)
        {
            return Login(new LoginCustomerWithAdminCredentialsModel() {Alias = customerAlias, Password = adminPassword, CustomerIdentifier = customerAlias}, "/Account/LoginCustomerWithAdminCredentials");
        }

        private ILoginResponseModel Login(LoginModel model, string url)
        {
            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, new UTF8Encoding(), "application/json");

            var response = InternalHttpClient.PostAsync(url, content).Result;
            var responseContent = response.Content.ReadAsStringAsync().Result;

            if (!response.IsSuccessStatusCode)
                throw new HttpResponseException(new HttpResponseMessage()
                { StatusCode = response.StatusCode, ReasonPhrase = $"Login gave error {response.ReasonPhrase} for user {model?.Alias ?? "null"} . Details: {responseContent}. Url: {url}" });

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
            if (response.Headers.TryGetValues("X-Paging-PreviousPage", out values))
                headers.PreviousPageLink = values.First();
            if (response.Headers.TryGetValues("X-Paging-NextPage", out values))
                headers.NextPageLink = values.First();
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
