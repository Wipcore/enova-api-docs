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

namespace Wipcore.eNova.Api.NETClient
{
    /// <summary>
    /// A repository for access to Enova through the API. 
    /// </summary>
    public class ApiRepository : IApiRepository
    {
        private readonly Func<IApiClient> _apiClientMaker;
        private readonly IConfigurationRoot _configuration;
        private readonly IServiceProvider _serviceProvider;
        private readonly ConcurrentDictionary<string, string> _properties = new ConcurrentDictionary<string, string>();
        private readonly ILogger _log;

        public ApiRepository(Func<IApiClient> apiClientMaker, IConfigurationRoot configuration, IServiceProvider serviceProvider, ILoggerFactory loggerFactory)
        {
            _apiClientMaker = apiClientMaker;
            _configuration = configuration;
            _serviceProvider = serviceProvider;
            _log = loggerFactory.CreateLogger(GetType().Namespace);
        }

        /// <summary>
        /// Check if object with given identifier exists.
        /// </summary>
        public bool ObjectExists<TModel>(string identifier) where TModel : BaseModel => ObjectExists<TModel>(identifier, 0);

        /// <summary>
        /// Check if object with given id exists.
        /// </summary>
        public bool ObjectExists<TModel>(int id) where TModel : BaseModel => ObjectExists<TModel>(null, id);

        private bool ObjectExists<TModel>(string identifier, int id) where TModel : BaseModel
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
            return id != 0 ? apiClient.ObjectExists(resource, id) : apiClient.ObjectExists(resource, identifier);
        }

        /// <summary>
        /// Get an object by id, serialized into TModel.
        /// </summary>
        public TModel GetObject<TModel>(int id, QueryModel queryModel = null, ContextModel contextModel = null, string action = null, IDictionary < string, object> extraParameters = null) where TModel : BaseModel 
            => GetObject<TModel>(null, id, queryModel, contextModel, action, extraParameters);
        

        /// <summary>
        /// Get an object by identifier, seralized into TModel. 
        /// </summary>
        public TModel GetObject<TModel>(string identifier, QueryModel queryModel = null, ContextModel contextModel = null, string action = null, IDictionary < string, object> extraParameters = null) where TModel : BaseModel 
            => GetObject<TModel>(identifier, 0, queryModel, contextModel, action, extraParameters);
        

        private TModel GetObject<TModel>(string identifier, int id, QueryModel queryModel = null, ContextModel contextModel = null, string action = null, IDictionary < string, object> extraParameters = null) where TModel : BaseModel
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

            var obj = id != 0 ? apiClient.GetOne(modelType, resource, id, queryModel: queryModel, contextModel:contextModel, extraParameters:extraParameters, action:action) :
                                apiClient.GetOne(modelType, resource, identifier, queryModel: queryModel, contextModel:contextModel, extraParameters:extraParameters, action:action);
            return (TModel)obj;
        }

        /// <summary>
        /// Get many objects, serialized into TModel. Query parameters, outputheaders, context/culture and which languages to query for are all optional.
        /// </summary>
        public IEnumerable<TModel> GetMany<TModel>(QueryModel queryModel = null, ApiResponseHeadersModel headers = null,ContextModel contextModel = null, 
            string action = null, List<string> languages = null, IDictionary<string, object> extraParameters = null)
        {
            string resource;
            var apiClient = GetManyInit(typeof(TModel), ref queryModel, languages, out resource);

            var objects = apiClient.GetMany<TModel>(resource, queryModel, headers: headers, contextModel: contextModel, extraParameters: extraParameters, action: action);
            return objects;
        }

        /// <summary>
        /// Get many objects, untyped. Query parameters, outputheaders, context/culture and which languages to query for are all optional.
        /// </summary>
        public IEnumerable<object> GetMany(Type modelType, QueryModel queryModel = null, ApiResponseHeadersModel headers = null, ContextModel contextModel = null, string action = null, List<string> languages = null, IDictionary<string, object> extraParameters = null)
        {
            string resource;
            var apiClient = GetManyInit(modelType, ref queryModel, languages, out resource);

            var objects = apiClient.GetMany(resource, queryModel, headers: headers, contextModel: contextModel, extraParameters: extraParameters, action: action);
            return objects;
        }

        private IApiClient GetManyInit(Type modelType, ref QueryModel queryModel, List<string> languages, out string resource)
        {
            var apiClient = _apiClientMaker.Invoke();
            var model = _serviceProvider.GetService(modelType) as BaseModel;

            if (model == null)
            {
                _log.LogWarning("Found no registered model for {0}. Using default.", modelType);
                model = (BaseModel) modelType.CreateInstance();
            }

            var registeredModelType = model.GetType();
            resource = model.GetResourceName();

            SetupQueryModel(ref queryModel, registeredModelType, languages);
            return apiClient;
        }

        /// <summary>
        /// Deletes an object by id.
        /// </summary>
        public bool DeleteObject<TModel>(int id) where TModel : BaseModel => DeleteObject<TModel>(id, null);

        /// <summary>
        /// Deletes an object by identifier.
        /// </summary>
        public bool DeleteObject<TModel>(string identifier) where TModel : BaseModel => DeleteObject<TModel>(0, identifier);

        private bool DeleteObject<TModel>(int id, string identifier) where TModel : BaseModel
        {
            var apiClient = _apiClientMaker.Invoke();
            var resource = ((TModel)_serviceProvider.GetService(typeof(TModel))).GetResourceName();

            var success = id != 0 ? apiClient.DeleteOne(resource, id) : apiClient.DeleteOne(resource, identifier);
            return success;
        }

        /// <summary>
        /// Saves an object. Specify specific action in url, responsetype to serialize the response into, and context/culture, as needed.
        /// </summary>
        public object SaveObject<TModel>(JObject jsonItem, string action = null, Type responseType = null, ContextModel contextModel = null, bool verifyIdentifierNotTaken = true, IDictionary<string, object> extraParameters = null) where TModel : BaseModel
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
            
            if(verifyIdentifierNotTaken)
                VerifyIdentifierNotTaken<TModel>(jsonItem, resource);

            var stringItem = JsonConvert.SerializeObject(jsonItem);
            var obj = apiClient.SaveOne(resource, stringItem, responseType ?? registeredModelType, action, contextModel:contextModel, extraParameters:extraParameters);
            
            return obj;
        }

        private void VerifyIdentifierNotTaken<TModel>(JObject jsonItem, string resource) where TModel : BaseModel
        {
            JToken id;
            if (jsonItem.TryGetValue("ID", StringComparison.CurrentCultureIgnoreCase, out id))
            {
                if (Convert.ToInt32(id) != 0)
                    return;//if id specified, then it won't become a new item regardless
            }

            JToken identifierToken;
            if (!jsonItem.TryGetValue("Identifier", StringComparison.CurrentCultureIgnoreCase, out identifierToken))
                return; //if no identifier, then fine

            var identifier = identifierToken.ToString();
            if (ObjectExists<TModel>(identifier))
                throw new HttpResponseException(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    ReasonPhrase = $"A {resource} with identifier {identifier} already exists."
                });
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
                _log.LogInformation("For type {0} {1} languages the following properties will be used in api request by default: {2}", type.Name, languagesToIndex == null ? "without" : "with", propertiesString);
                return propertiesString;
            });
        }
    }
}
