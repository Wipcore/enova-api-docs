using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Wipcore.Enova.Api.Abstractions;
using Wipcore.Enova.Api.Abstractions.Interfaces;
using Wipcore.Enova.Api.Abstractions.Models;

namespace Wipcore.Enova.Api.NetClient
{
    public class ApiClientAsync : IApiClientAsync
    {
        private const bool ContinueOnCapturedContext = false;
        public const string TokenKey = "ApiToken";
        private readonly IConfigurationRoot _configuration;
        private readonly IHttpContextAccessor _httpAccessor;

        private readonly IDataProtector _protector;
        private readonly ILogger _log;

        public ApiClientAsync(IConfigurationRoot configuration, IHttpContextAccessor httpAccessor, IDataProtectionProvider protectionProvider, ILoggerFactory loggerFactory)
        {
            _configuration = configuration;
            _httpAccessor = httpAccessor;
            _protector = protectionProvider.CreateProtector("CookieEncrypter");
            InternalHttpClient = new HttpClient { BaseAddress = new Uri(configuration["API:Url"] ?? "http://localhost:5000/api/") };
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
        public async Task<bool> ObjectExistsAsync(string controller, string identifier)
        {
            identifier = identifier ?? String.Empty;
            var url = $"{controller}/{identifier}";
            var response = await InternalHttpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, url)).ConfigureAwait(ContinueOnCapturedContext);
            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(ContinueOnCapturedContext);

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
        public async Task<bool> ObjectExistsAsync(string controller, int id) => await ObjectExistsAsync(controller, "id-" + id).ConfigureAwait(ContinueOnCapturedContext);

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
        public async Task<object> GetOneAsync(Type resonseType, string controller, int id, string action = null, QueryModel queryModel = null,
            IContextModel contextModel = null, IDictionary<string, object> extraParameters = null, ApiResponseHeadersModel headers = null, bool throwIfNotFound = false)
        {
            return await GetOneAsync(resonseType, controller, "id-" + id, action, queryModel, contextModel, extraParameters, headers, throwIfNotFound).ConfigureAwait(ContinueOnCapturedContext);
        }
        
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
        public async Task<TModel> GetOneAsync<TModel>(string controller, string identifier = null, string action = null, QueryModel queryModel = null, IContextModel contextModel = null, 
            IDictionary<string, object> extraParameters = null, ApiResponseHeadersModel headers = null, bool throwIfNotFound = false) where TModel : class
        {
            return (TModel) await GetOneAsync(typeof(TModel), controller, identifier, action, queryModel, contextModel, extraParameters, headers, throwIfNotFound).ConfigureAwait(ContinueOnCapturedContext);
        }

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
        public async Task<TModel> GetOneAsync<TModel>(string controller, int id, string action = null, QueryModel queryModel = null,
            IContextModel contextModel = null, IDictionary<string, object> extraParameters = null, ApiResponseHeadersModel headers = null,
            bool throwIfNotFound = false) where TModel : class
        {
            return (TModel) await GetOneAsync(typeof(TModel), controller, "id-" + id, action, queryModel, contextModel, extraParameters, headers, throwIfNotFound).ConfigureAwait(ContinueOnCapturedContext);
        }

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
        public async Task<object> GetOneAsync(Type resonseType, string controller, string identifier = null, string action = null, QueryModel queryModel = null,
            IContextModel contextModel = null, IDictionary<string, object> extraParameters = null, ApiResponseHeadersModel headers = null, bool throwIfNotFound = false)
        {
            action = action == null ? string.Empty : "/" + action;
            identifier = identifier ?? String.Empty;
            var url = $"{controller}/{identifier}{action}{BuildParameters(contextModel, queryModel, extraParameters)}";
            var response = await InternalHttpClient.GetAsync(url).ConfigureAwait(ContinueOnCapturedContext);
            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(ContinueOnCapturedContext);

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
        /// <param name="controller">The controller path of the the request, i.e. "product" in /api/product/</param>
        /// <param name="queryModel">Paging, size, sort, filter etc.</param>
        /// <param name="contextModel">Context/culture values such as language.</param>
        /// <param name="action">Action part of the url if there is one.</param>
        /// <param name="extraParameters">Any extra query parameters.</param>
        /// <param name="headers">Recived headers if object is given.</param>
        public async Task<IEnumerable<object>> GetManyAsync(string controller, IQueryModel queryModel = null, IContextModel contextModel = null,
            string action = null, IDictionary<string, object> extraParameters = null, ApiResponseHeadersModel headers = null)
        {
            return await GetManyAsync<object>(controller, queryModel, contextModel, action, extraParameters, headers).ConfigureAwait(ContinueOnCapturedContext);
        }

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
        public async Task<IEnumerable<TModel>> GetManyAsync<TModel>(string controller, IQueryModel queryModel = null, IContextModel contextModel = null,
            string action = null, IDictionary<string, object> extraParameters = null, ApiResponseHeadersModel headers = null)
        {
            action = action == null ? string.Empty : "/" + action;
            var url = $"{controller}{action}{BuildParameters(contextModel, queryModel, extraParameters)}";
            return await GetManyRequestAsync<TModel>(headers, url).ConfigureAwait(ContinueOnCapturedContext);
        }

        /// <summary>
        /// Get the next page of items from a previous request.
        /// </summary>
        /// <typeparam name="TModel">The type to serialize the items into.</typeparam>
        /// <param name="headersOfPreviousRequest">Headers received from previous request.</param>
        public async Task<IEnumerable<TModel>> GetNextPageAsync<TModel>(ApiResponseHeadersModel headersOfPreviousRequest)
        {
            var url = headersOfPreviousRequest.NextPageLink;
            return await GetManyRequestAsync<TModel>(headersOfPreviousRequest, url).ConfigureAwait(ContinueOnCapturedContext);
        }

        /// <summary>
        /// Get the previous page of items from a previous request.
        /// </summary>
        /// <typeparam name="TModel">The type to serialize the items into.</typeparam>
        /// <param name="headersOfPreviousRequest">Headers received from previous request.</param>
        public async Task<IEnumerable<TModel>> GetPreviousPageAsync<TModel>(ApiResponseHeadersModel headersOfPreviousRequest)
        {
            var url = headersOfPreviousRequest.PreviousPageLink;
            return await GetManyRequestAsync<TModel>(headersOfPreviousRequest, url).ConfigureAwait(ContinueOnCapturedContext);
        }

        /// <summary>
        /// Delete one object.
        /// </summary>
        /// <param name="controller">The controller path of the the request, i.e. "product" in /api/product/</param>
        /// <param name="id">ID of the object to delete.</param>
        /// <param name="throwIfNotFound">Set to true to throw exception if object does not exist.</param>
        public async Task<bool> DeleteOneAsync(string controller, int id, bool throwIfNotFound = false)
        {
            var url = $"{controller}/id-{id}";
            var response = await InternalHttpClient.DeleteAsync(url).ConfigureAwait(ContinueOnCapturedContext);

            if (response.IsSuccessStatusCode)
                return true;

            if (!throwIfNotFound && response.StatusCode == HttpStatusCode.NotFound)
                return false;

            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(ContinueOnCapturedContext);
            throw new HttpResponseException(new HttpResponseMessage()
            {
                StatusCode = response.StatusCode,
                ReasonPhrase = $"Delete url {InternalHttpClient.BaseAddress}{url} gave error: {response.ReasonPhrase}. Details: {responseContent}"
            });
        }

        /// <summary>
        /// Delete one object.
        /// </summary>
        /// <param name="controller">The controller path of the the request, i.e. "product" in /api/product/</param>
        /// <param name="identifier">Identifier of the object to delete.</param>
        /// <param name="throwIfNotFound">Set to true to throw exception if object does not exist.</param>
        public async Task<bool> DeleteOneAsync(string controller, string identifier, bool throwIfNotFound = false)
        {
            var url = $"{controller}/{identifier}";
            var response = await InternalHttpClient.DeleteAsync(url).ConfigureAwait(ContinueOnCapturedContext);

            if (response.IsSuccessStatusCode)
                return true;

            if (!throwIfNotFound && response.StatusCode == HttpStatusCode.NotFound)
                return false;

            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(ContinueOnCapturedContext);
            throw new HttpResponseException(new HttpResponseMessage()
            {
                StatusCode = response.StatusCode,
                ReasonPhrase = $"Delete url {InternalHttpClient.BaseAddress}{url} gave error: {response.ReasonPhrase}. Details: {responseContent}"
            });
        }

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
        public async Task<object> SaveOneAsync(string controller, string item, Type responseType, string action = null,
            IContextModel contextModel = null, IDictionary<string, object> extraParameters = null, bool put = true)
        {
            action = action == null ? string.Empty : action + "/";
            var url = $"{controller}/{action}{BuildParameters(contextModel, null, extraParameters)}";
            var stringContent = new StringContent(item, Encoding.UTF8, "application/json");
            var response = put ? await InternalHttpClient.PutAsync(url, stringContent).ConfigureAwait(ContinueOnCapturedContext) : await InternalHttpClient.PostAsync(url, stringContent).ConfigureAwait(ContinueOnCapturedContext);
            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(ContinueOnCapturedContext);

            if (!response.IsSuccessStatusCode)
                throw new HttpResponseException(new HttpResponseMessage()
                {
                    StatusCode = response.StatusCode,
                    ReasonPhrase = $"Save url {InternalHttpClient.BaseAddress}{url}  gave error: {response.ReasonPhrase}. Details: {responseContent}"
                });
            
            var model = JsonConvert.DeserializeObject(responseContent, responseType);
            return model;
        }

        /// <summary>
        /// Returns true if enova is responding through the API.
        /// </summary>
        public async Task<bool> IsEnovaAliveAsync()
        {
            try
            {
                var response = await InternalHttpClient.GetAsync("/IsEnovaAlive").ConfigureAwait(ContinueOnCapturedContext);
                return Convert.ToBoolean(await response.Content.ReadAsStringAsync().ConfigureAwait(ContinueOnCapturedContext));
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
        public async Task<bool> IsLoggedInAsync()
        {
            var dictionary = await GetLoggedInUserInfoAsync().ConfigureAwait(ContinueOnCapturedContext);
            return Convert.ToBoolean(dictionary["LoggedIn"]);
        }

        /// <summary>
        /// Get information about the currently logged in user.
        /// </summary>
        public async Task<IDictionary<string, string>> GetLoggedInUserInfoAsync()
        {
            var response = await InternalHttpClient.GetAsync("/Account/LoggedInAs").ConfigureAwait(ContinueOnCapturedContext);
            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(ContinueOnCapturedContext);

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
        public async Task<IDictionary<string, string>> GetEnovaNodeInfoAsync()
        {
            var response = await InternalHttpClient.GetAsync("/NodeInfo").ConfigureAwait(ContinueOnCapturedContext);
            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(ContinueOnCapturedContext);

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
        public async Task<bool> LogoutAsync()
        {
            var response = await InternalHttpClient.PostAsync("/Account/Logout", null).ConfigureAwait(ContinueOnCapturedContext);
            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(ContinueOnCapturedContext);
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
        public async Task<ILoginResponseModel> LoginAdminAsync(string alias, string password)
        {
            return await LoginAsync(new LoginModel() { Alias = alias, Password = password }, "/Account/LoginAdmin").ConfigureAwait(ContinueOnCapturedContext);
        }

        /// <summary>
        /// Login a customer
        /// </summary>
        public async Task<ILoginResponseModel> LoginCustomerAsync(string alias, string password)
        {
            return await LoginAsync(new LoginModel() { Alias = alias, Password = password }, "/Account/LoginCustomer").ConfigureAwait(ContinueOnCapturedContext);
        }

        /// <summary>
        /// Login a customer as admin
        /// </summary>
        public async Task<ILoginResponseModel> LoginCustomerAsAdminAsync(string customerAlias, string adminAlias, string adminPassword)
        {
            return await LoginAsync(new LoginCustomerWithAdminCredentialsModel() { Alias = customerAlias, Password = adminPassword, CustomerIdentifier = customerAlias }, 
                "/Account/LoginCustomerWithAdminCredentials").ConfigureAwait(ContinueOnCapturedContext);
        }
        

        private async Task<ILoginResponseModel> LoginAsync(LoginModel model, string url)
        {
            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, new UTF8Encoding(), "application/json");

            var response = await InternalHttpClient.PostAsync(url, content).ConfigureAwait(ContinueOnCapturedContext);
            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(ContinueOnCapturedContext);

            if (!response.IsSuccessStatusCode)
                throw new HttpResponseException(new HttpResponseMessage()
                { StatusCode = response.StatusCode, ReasonPhrase = $"Login gave error {response.ReasonPhrase} for user {model?.Alias ?? "null"} . Details: {responseContent}. Url: {url}" });

            var loginModel = JsonConvert.DeserializeObject<LoginResponseModel>(responseContent);

            var httpContext = _httpAccessor.HttpContext;
            if (!String.IsNullOrEmpty(loginModel.AccessToken))
            {
                //if elastic thread, then this context is internal within the app, thus set directly on next request. Otherwise send to client as a response
                if (httpContext.TraceIdentifier == WipConstants.ElasticIndexHttpContextIdentifier || httpContext.TraceIdentifier == WipConstants.ElasticDeltaIndexHttpContextIdentifier || httpContext.TraceIdentifier == WipConstants.InternalHttpContextIdentifier)
                    httpContext.Request.Cookies = new RequestCookieCollection(new Dictionary<string, string>() { { TokenKey, loginModel.AccessToken } });
                else
                    httpContext.Response.Cookies.Append(TokenKey, loginModel.AccessToken);
            }

            if (loginModel.UserId != null)
            {
                //add the id of the user in another cookie. Useful for getting the current users data (like filters) without querying api every time.
                var protectedId = _protector.Protect(loginModel.UserId.Value.ToString());
                var expireTime = DateTime.Now.AddMinutes( _configuration.GetValue("Auth:ExpireTimeMinutes", 120));
                httpContext.Response.Cookies.Append(WipConstants.UserIdCookieIdentifier, protectedId, new CookieOptions() {Expires = expireTime });
            }

            return loginModel;
        }

        private async Task<IEnumerable<TModel>> GetManyRequestAsync<TModel>(ApiResponseHeadersModel headers, string url)
        {
            var response = await InternalHttpClient.GetAsync(url).ConfigureAwait(ContinueOnCapturedContext);
            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(ContinueOnCapturedContext);

            if (!response.IsSuccessStatusCode)
                throw new HttpResponseException(new HttpResponseMessage()
                {
                    StatusCode = response.StatusCode,
                    ReasonPhrase = $"Get url {InternalHttpClient.BaseAddress}{url}  gave error: {response.ReasonPhrase}. Details: {responseContent}"
                });

            SetResponseHeaders(response, headers);
            var models = JsonConvert.DeserializeObject<IEnumerable<TModel>>(responseContent);

            return models;
        }

        private void SetResponseHeaders(HttpResponseMessage response, ApiResponseHeadersModel headers)
        {
            if (headers == null)
                return;

            if (response.Headers.TryGetValues("X-Paging-PageNo", out var values))
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
