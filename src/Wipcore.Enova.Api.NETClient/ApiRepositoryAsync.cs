using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Fasterflect;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Wipcore.Enova.Api.Abstractions.Attributes;
using Wipcore.Enova.Api.Abstractions.Interfaces;
using Wipcore.Enova.Api.Abstractions.Models;
using Wipcore.Enova.Api.Abstractions.Models.EnovaTypes;

namespace Wipcore.Enova.Api.NetClient
{
    /// <summary>
    /// A repository for async access to Enova through the API.
    /// </summary>
    public class ApiRepositoryAsync : IApiRepositoryAsync
    {
        private const bool ContinueOnCapturedContext = false;
        private readonly Func<IApiClientAsync> _apiClientMaker;
        private readonly IConfigurationRoot _configuration;
        private readonly IServiceProvider _serviceProvider;
        private readonly ConcurrentDictionary<string, string> _properties = new ConcurrentDictionary<string, string>();
        private readonly ILogger _log;

        public ApiRepositoryAsync(Func<IApiClientAsync> apiClientMaker, IConfigurationRoot configuration, IServiceProvider serviceProvider, ILoggerFactory loggerFactory)
        {
            _apiClientMaker = apiClientMaker;
            _configuration = configuration;
            _serviceProvider = serviceProvider;
            _log = loggerFactory.CreateLogger(GetType().Namespace);
        }


        /// <summary>
        /// Check if object with given identifier exists.
        /// </summary>
        public async Task<bool> ObjectExistsAsync<TModel>(string identifier) where TModel : BaseModel => await ObjectExistsAsync<TModel>(identifier, 0).ConfigureAwait(ContinueOnCapturedContext);


        /// <summary>
        /// Check if object with given id exists.
        /// </summary>
        public async Task<bool> ObjectExistsAsync<TModel>(int id) where TModel : BaseModel => await ObjectExistsAsync<TModel>(null, id).ConfigureAwait(ContinueOnCapturedContext);


        private async Task<bool> ObjectExistsAsync<TModel>(string identifier, int id) where TModel : BaseModel
        {
            var apiClient = _apiClientMaker.Invoke();
            var model = _serviceProvider.GetService(typeof(TModel)) as TModel;

            if (model == null)
            {
                _log.LogWarning("Found no registered model for {0}. Using default.", typeof(TModel));
                model = (TModel)typeof(TModel).CreateInstance();
            }

            if (String.IsNullOrEmpty(identifier) && id == 0)
                return false;

            var resource = model.GetResourceName();
            return id != 0 ? await apiClient.ObjectExistsAsync(resource, id).ConfigureAwait(ContinueOnCapturedContext) : await apiClient.ObjectExistsAsync(resource, identifier).ConfigureAwait(ContinueOnCapturedContext);
        }

        /// <summary>
        /// Get an object by id, serialized into TModel.
        /// </summary>
        public async Task<TModel> GetObjectAsync<TModel>(int id, QueryModel queryModel = null, ContextModel contextModel = null,
            string action = null, IDictionary<string, object> extraParameters = null) where TModel : BaseModel
            => await GetObjectAsync<TModel>(null, id, queryModel, contextModel, action, extraParameters).ConfigureAwait(ContinueOnCapturedContext);

        /// <summary>
        /// Get an object by identifier, seralized into TModel. 
        /// </summary>
        public async Task<TModel> GetObjectAsync<TModel>(string identifier, QueryModel queryModel = null, ContextModel contextModel = null,
            string action = null, IDictionary<string, object> extraParameters = null) where TModel : BaseModel
            => await GetObjectAsync<TModel>(identifier, 0, queryModel, contextModel, action, extraParameters).ConfigureAwait(ContinueOnCapturedContext);

        private async Task<TModel> GetObjectAsync<TModel>(string identifier, int id, QueryModel queryModel = null, ContextModel contextModel = null, string action = null, IDictionary<string, object> extraParameters = null) where TModel : BaseModel
        {
            var apiClient = _apiClientMaker.Invoke();
            var model = _serviceProvider.GetService(typeof(TModel)) as TModel;

            if (model == null)
            {
                _log.LogWarning("Found no registered model for {0}. Using default.", typeof(TModel));
                model = (TModel)typeof(TModel).CreateInstance();
            }

            if (String.IsNullOrEmpty(identifier) && id == 0) //must have id or identifier
                return model;

            var modelType = model.GetType();

            var resource = model.GetResourceName();
            SetupQueryModel(ref queryModel, modelType, null);

            var obj = id != 0 ? await apiClient.GetOneAsync(modelType, resource, id, queryModel: queryModel, contextModel: contextModel, extraParameters: extraParameters, action: action).ConfigureAwait(ContinueOnCapturedContext) :
                await apiClient.GetOneAsync(modelType, resource, identifier, queryModel: queryModel, contextModel: contextModel, extraParameters: extraParameters, action: action).ConfigureAwait(ContinueOnCapturedContext);
            return (TModel)obj;
        }

        /// <summary>
        /// Get many objects, untyped. Query parameters, outputheaders, context/culture and which languages to query for are all optional.
        /// </summary>
        public async Task<IEnumerable<TModel>> GetManyAsync<TModel>(QueryModel queryModel = null, ApiResponseHeadersModel headers = null,
            ContextModel contextModel = null, string action = null, List<string> languages = null, IDictionary<string, object> extraParameters = null)
        {
            var (apiClient, resource) = GetManyInit(typeof(TModel), ref queryModel, languages);

            var objects = await apiClient.GetManyAsync<TModel>(resource, queryModel, headers: headers, contextModel: contextModel, extraParameters: extraParameters, action: action).ConfigureAwait(ContinueOnCapturedContext);
            return objects;
        }

        /// <summary>
        /// Get many objects, serialized into TModel. Query parameters, outputheaders, context/culture and which languages to query for are all optional.
        /// </summary>
        public async Task<IEnumerable<object>> GetManyAsync(Type modelType, QueryModel queryModel = null, ApiResponseHeadersModel headers = null,
            ContextModel contextModel = null, string action = null, List<string> languages = null, IDictionary<string, object> extraParameters = null)
        {
            var (apiClient, resource) = GetManyInit(modelType, ref queryModel, languages);

            var objects = await apiClient.GetManyAsync(resource, queryModel, headers: headers, contextModel: contextModel, extraParameters: extraParameters, action: action).ConfigureAwait(ContinueOnCapturedContext);
            return objects;
        }

        private (IApiClientAsync apiClient, string resource) GetManyInit(Type modelType, ref QueryModel queryModel, List<string> languages)
        {
            var apiClient = _apiClientMaker.Invoke();
            var model = _serviceProvider.GetService(modelType) as BaseModel;

            if (model == null)
            {
                _log.LogWarning("Found no registered model for {0}. Using default.", modelType);
                model = (BaseModel)modelType.CreateInstance();
            }

            var registeredModelType = model.GetType();
            var resource = model.GetResourceName();

            SetupQueryModel(ref queryModel, registeredModelType, languages);
            return (apiClient, resource);
        }

        /// <summary>
        /// Get the next page of items from a previous request.
        /// </summary>
        public async Task<IEnumerable<TModel>> GetNextPageAsync<TModel>(ApiResponseHeadersModel headersOfPreviousRequest)
            => await _apiClientMaker.Invoke().GetNextPageAsync<TModel>(headersOfPreviousRequest).ConfigureAwait(ContinueOnCapturedContext);

        /// <summary>
        /// Get the previous page of items from a previous request.
        /// </summary>
        public async Task<IEnumerable<TModel>> GetPreviousPageAsync<TModel>(ApiResponseHeadersModel headersOfPreviousRequest)
            => await _apiClientMaker.Invoke().GetPreviousPageAsync<TModel>(headersOfPreviousRequest).ConfigureAwait(ContinueOnCapturedContext);

        /// <summary>
        /// Deletes an object by id.
        /// </summary>
        public async Task<bool> DeleteObjectAsync<TModel>(int id) where TModel : BaseModel
            => await DeleteObjectAsync<TModel>(id, null).ConfigureAwait(ContinueOnCapturedContext);

        /// <summary>
        /// Deletes an object by identifier.
        /// </summary>
        public async Task<bool> DeleteObjectAsync<TModel>(string identifier) where TModel : BaseModel
            => await DeleteObjectAsync<TModel>(0, identifier).ConfigureAwait(ContinueOnCapturedContext);

        private async Task<bool> DeleteObjectAsync<TModel>(int id, string identifier) where TModel : BaseModel
        {
            var apiClient = _apiClientMaker.Invoke();
            var resource = ((TModel)_serviceProvider.GetService(typeof(TModel))).GetResourceName();

            var success = id != 0 ? await apiClient.DeleteOneAsync(resource, id).ConfigureAwait(ContinueOnCapturedContext) : await apiClient.DeleteOneAsync(resource, identifier).ConfigureAwait(ContinueOnCapturedContext);
            return success;
        }

        /// <summary>
        /// Saves an object. Specify specific action in url, responsetype to serialize the response into, and context/culture, as needed.
        /// </summary>
        public async Task<object> SaveObjectAsync<TModel>(JObject jsonItem, string action = null, Type responseType = null,
            ContextModel contextModel = null, bool verifyIdentifierNotTaken = true, IDictionary<string, object> extraParameters = null) where TModel : BaseModel
        {
            var apiClient = _apiClientMaker.Invoke();
            var model = _serviceProvider.GetService(typeof(TModel)) as TModel;

            if (model == null)
            {
                _log.LogWarning("Found no registered model for {0}. Using default.", typeof(TModel));
                model = (TModel)typeof(TModel).CreateInstance();
            }

            var registeredModelType = model.GetType();
            var resource = model.GetResourceName();

            if (verifyIdentifierNotTaken)
                await VerifyIdentifierNotTaken<TModel>(jsonItem, resource).ConfigureAwait(ContinueOnCapturedContext);

            var stringItem = JsonConvert.SerializeObject(jsonItem);
            var obj = await apiClient.SaveOneAsync(resource, stringItem, responseType ?? registeredModelType, action, contextModel, extraParameters).ConfigureAwait(ContinueOnCapturedContext);

            return obj;
        }

        private async Task VerifyIdentifierNotTaken<TModel>(JObject jsonItem, string resource) where TModel : BaseModel
        {
            if (jsonItem.TryGetValue("ID", StringComparison.CurrentCultureIgnoreCase, out var id))
            {
                if (Convert.ToInt32(id) != 0)
                    return;//if id specified, then it won't become a new item regardless
            }

            if (!jsonItem.TryGetValue("Identifier", StringComparison.CurrentCultureIgnoreCase, out var identifierToken))
                return; //if no identifier, then fine

            var identifier = identifierToken.ToString();
            if (await ObjectExistsAsync<TModel>(identifier).ConfigureAwait(ContinueOnCapturedContext))
            {
                //throw new HttpResponseException(new HttpResponseMessage
                //{
                //    StatusCode = HttpStatusCode.BadRequest,
                //    ReasonPhrase = $"A {resource} with identifier {identifier} already exists."
                //});
                throw new Exception($"A {resource} with identifier {identifier} already exists.");
            }
                
        }

        /// <summary>
        /// Gets default query parameters if non set.
        /// </summary>
        private void SetupQueryModel(ref QueryModel queryModel, Type type, List<string> languagesToIndex)
        {
            if (queryModel == null)
                queryModel = new QueryModel();
            if (queryModel.Properties == null || !queryModel.Properties.Any())
                queryModel.Properties = GetProperties(type, languagesToIndex);
        }

        /// <summary>
        /// Get default properties, with or without specifying which languages to parse into. Resolves once and saves in dictionary.
        /// </summary>
        private string GetProperties(Type type, List<string> languagesToIndex)
        {
            return _properties.GetOrAdd(type.Name + (languagesToIndex == null ? "" : "_lang"), (t) =>
            {
                var properties = new List<string>();
                var modelProperties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (var modelProperty in modelProperties)
                {
                    var propPres = modelProperty.GetCustomAttribute<PropertyPresentation>();
                    var languageProperty = propPres?.LanguageDependant == true;

                    if (languageProperty && languagesToIndex != null)
                    {
                        if (modelProperty.PropertyType.Namespace == typeof(object).Namespace) //base type
                        {
                            foreach (var language in languagesToIndex)
                            {
                                properties.Add($"{modelProperty.Name}-{language}");
                            }
                        }
                        else
                        {
                            properties.Add($"{modelProperty.Name}-{String.Join(";", languagesToIndex)}");
                        }

                    }
                    else
                    {
                        properties.Add(modelProperty.Name);
                    }
                }

                var propertiesString = String.Join(",", properties);
                _log.LogInformation("For type {0} {1} languages the following properties will be used in api request by default: {2}", 
                    type.Name, languagesToIndex == null ? "without" : "with", propertiesString);

                return propertiesString;
            });
        }
    }
}
