using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using Fasterflect;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Wipcore.Core;
using Wipcore.Core.SessionObjects;
using Wipcore.eNova.Api.WebApi.Services;
using Wipcore.Enova.Api.Abstractions;
using Wipcore.Enova.Api.Abstractions.Interfaces;
using Wipcore.Enova.Api.Abstractions.Internal;
using Wipcore.Enova.Api.WebApi.Helpers;
using Wipcore.Enova.Core;
using Wipcore.Enova.Generics;

namespace Wipcore.Enova.Api.WebApi.Services
{
    /// <summary>
    /// Service for getting and saving objects to Enova.
    /// </summary>
    public class ObjectService : IObjectService
    {
        private readonly IPagingService _pagingService;
        private readonly ISortService _sortService;
        private readonly IFilterService _filterService;
        private readonly IMappingFromEnovaService _mappingFromEnovaService;
        private readonly IMappingToEnovaService _mappingToEnovaService;
        private readonly ITemplateService _templateService;
        private readonly IContextService _contextService;
        private readonly IAuthService _authService;
        private readonly ICacheService _cacheService;
        private readonly ILogger _logger;
        private readonly bool _defaultProductsToNotStockable;


        public ObjectService(IPagingService pagingService, ISortService sortService, IFilterService filterService, IMappingFromEnovaService mappingFromEnovaService, IMappingToEnovaService mappingToEnovaService, 
            ITemplateService templateService, IContextService contextService, ILoggerFactory loggerFactory, IAuthService authService, ICacheService cacheService, IConfigurationRoot configuration)
        {
            _pagingService = pagingService;
            _sortService = sortService;
            _filterService = filterService;
            _mappingFromEnovaService = mappingFromEnovaService;
            _mappingToEnovaService = mappingToEnovaService;
            _templateService = templateService;
            _contextService = contextService;
            _authService = authService;
            _cacheService = cacheService;
            _logger = loggerFactory.CreateLogger(GetType().Name);
            _defaultProductsToNotStockable = configuration.GetValue<bool>("EnovaSettings:DefaultProductsToNotStockable", true);
        }


        /// <summary>
        /// Returns true if object with identifier was found.
        /// </summary>
        public bool Exists<T>(string identifier)
        {
            var context = _contextService.GetContext();
            return context.FindObject(identifier, typeof(T), throwExceptionIfNotFound: false) != null;
        }

        /// <summary>
        /// Returns true if object with id was found.
        /// </summary>
        public bool Exists<T>(int id)
        {
            var context = _contextService.GetContext();
            return context.FindObject(id, typeof(T), throwExceptionIfNotFound: false) != null;
        }

        /// <summary>
        /// Get an objects from Enova. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestContext">Context for the query, ie language.</param>
        /// <param name="query">Query parameters.</param>
        /// <param name="identifier">Object identifier.</param>
        /// <returns></returns>
        public IDictionary<string, object> Get<T>(IContextModel requestContext, IQueryModel query, string identifier) where T : BaseObject
        {
            var derivedType = typeof(T).GetMostDerivedEnovaType();
            var context = _contextService.GetContext();
            query = _templateService.GetQueryModelFromTemplateConfiguration(derivedType, query);

            var obj = context.FindObject(identifier, typeof(T), throwExceptionIfNotFound: true);
            return _mappingFromEnovaService.MapFromEnovaObject(obj, query.Properties);
        }

        /// <summary>
        /// Get an objects from Enova. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestContext">Context for the query, ie language.</param>
        /// <param name="query">Query parameters.</param>
        /// <param name="id">Object id.</param>
        /// <returns></returns>
        public IDictionary<string, object> Get<T>(IContextModel requestContext, IQueryModel query, int id) where T : BaseObject
        {
            var derivedType = typeof(T).GetMostDerivedEnovaType();
            var context = _contextService.GetContext();
            query = _templateService.GetQueryModelFromTemplateConfiguration(derivedType, query);

            var obj = context.FindObject(id, typeof(T), throwExceptionIfNotFound: true);
            return _mappingFromEnovaService.MapFromEnovaObject(obj, query.Properties);
        }

        /// <summary>
        /// Get several objects from Enova.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestContext">Context for the query, ie language.</param>
        /// <param name="query">Query parameters.</param>
        /// <param name="ids">List of ids of objects to find.</param>
        public IEnumerable<IDictionary<string, object>> GetMany<T>(IContextModel requestContext, IQueryModel query, IEnumerable<int> ids) where T : BaseObject
        {
            return this.Get<T>(requestContext, query, ids, null);
        }

        /// <summary>
        /// Get several objects from Enova.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestContext">Context for the query, ie language.</param>
        /// <param name="query">Query parameters.</param>
        /// <param name="identifiers">List of identifiers of objects to find.</param>
        public IEnumerable<IDictionary<string, object>> GetMany<T>(IContextModel requestContext, IQueryModel query, IEnumerable<string> identifiers) where T : BaseObject
        {
            return this.Get<T>(requestContext, query, null, identifiers);
        }

        private IEnumerable<IDictionary<string, object>> Get<T>(IContextModel requestContext, IQueryModel query, IEnumerable<int> ids, IEnumerable<string> identifiers) where T : BaseObject
        {
            var derivedType = typeof(T).GetMostDerivedEnovaType();
            var context = _contextService.GetContext();
            var objectList = ids != null ? context.FindObjects(ids, derivedType) : context.FindObjects(identifiers, derivedType);
            return this.GetMany<T>(requestContext, query, objectList);
        }

        /// <summary>
        /// Get objects from Enova. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestContext">Context for the query, ie language.</param>
        /// <param name="query">Query parameters.</param>
        /// <param name="candidates">Objects to look at, or null to look at all objects.</param>
        /// <returns></returns>
        public IEnumerable<IDictionary<string, object>> GetMany<T>(IContextModel requestContext, IQueryModel query, BaseObjectList candidates = null) where T : BaseObject
        {
            var derivedType = typeof(T).GetMostDerivedEnovaType();
            query = _templateService.GetQueryModelFromTemplateConfiguration(derivedType, query);

            var cache = _cacheService.GetCache(requestContext, query, derivedType, candidates);
            if (cache != null)
                return cache;
            
            var context = _contextService.GetContext();
            var memoryObject = IsMemoryObject(derivedType);

            //return from the candidates, or if memoryobject, get whats in memory. otherwise search the database
            var objectList = candidates ?? (memoryObject ? context.GetAllObjects(derivedType) :
                             context.Search(query.Filter ?? "ID > 0", derivedType, null, 0, null, false));

            objectList = _sortService.Sort(objectList, query.Sort);
            objectList = _filterService.Filter(objectList, query.Filter);
            objectList = _pagingService.Page(objectList, query.Page.Value, query.Size.Value);
            var objects = _mappingFromEnovaService.MapFromEnovaObject(objectList, query.Properties).ToList();

            _cacheService.SetCache(objects, requestContext, query, derivedType, candidates);

            return objects;
        }

        /// <summary>
        /// Save an object to Enova with the given values.
        /// </summary>
        /// <typeparam name="T">The most derived type of T is saved.</typeparam>
        /// <param name="requestContext">Context for the query, ie language.</param>
        /// <param name="values">Properties to save on the object.</param>
        public IDictionary<string, object> Save<T>(IContextModel requestContext, Dictionary<string, object> values) where T : BaseObject
        {
            if (values == null)
                return null;

            var newObject = false;
            var context = _contextService.GetContext();

            var id = values.GetValueInsensitive<int>("id");
            var identifier = values.GetValueInsensitive<string>("identifier");
            
            var obj = id != 0 ? context.FindObject<T>(id) : context.FindObject<T>(identifier);

            if (obj == null)
            {
                obj = EnovaObjectCreationHelper.CreateNew<T>(context);
                newObject = true;

                var product = obj as EnovaBaseProduct;
                if (_defaultProductsToNotStockable && product != null)
                    product.IsNotStockable = true;
            }
            else
                obj.Edit();

            var postSaveMappers = _mappingToEnovaService.MapToEnovaObject(obj, values);
            obj.Save();
            
            postSaveMappers?.ForEach(x => x.Invoke());

            _logger.LogInformation("{0} {1} object with Identifier: {2} of Type: {3} with Values: {4}", _authService.LogUser(), newObject ? "Created" : "Updated", identifier, obj.GetType().Name, values.ToLog());

            _cacheService.ClearCache(obj.GetType());

            var changedObject = context.FindObject<T>(obj.ID);//reget from enova to get any changes made
            return _mappingFromEnovaService.MapFromEnovaObject(changedObject, String.Join(",",values.Select(x => x.Key)));//remap to get changed values
        }

        /// <summary>
        /// Calculate the effects of mapping an object without actually saving it. Useful for carts.
        /// </summary>
        /// <typeparam name="T">The most derived type of T is calculated.</typeparam>
        /// <param name="requestContext">Context for the query, ie language.</param>
        /// <param name="values">Properties to calculate on the object.</param>
        public IDictionary<string, object> Calculate<T>(IContextModel requestContext, Dictionary<string, object> values) where T : BaseObject
        {
            if (values == null)
                return null;

            var context = _contextService.GetContext();
            var id = values.GetValueInsensitive<int>("id");
            var identifier = values.GetValueInsensitive<string>("identifier");

            var obj = id != 0 ? context.FindObject<T>(id) : context.FindObject<T>(identifier);

            if (obj == null)
                obj = EnovaObjectCreationHelper.CreateNew<T>(context);
            else
                obj.Edit();

            _mappingToEnovaService.MapToEnovaObject(obj, values); //ignore post save mappers when not saving

            return _mappingFromEnovaService.MapFromEnovaObject(obj, String.Join(",", values.Select(x => x.Key)));//remap to get changed values
        }


        /// <summary>
        /// Deletes an object of type T and given id from Enova. Returns true if successfull. False if the object did not exist.
        /// </summary>
        public bool Delete<T>(int id) where T : BaseObject
        {
            var context = _contextService.GetContext();
            var obj = context.FindObject<T>(id);

            if (obj == null)
                return false;

            _cacheService.ClearCache(obj.GetType());
            obj.Delete();

            _logger.LogInformation($"{_authService.LogUser()} deleted object with ID {obj.ID} and identifier {obj.Identifier} of type {obj.GetType().Name}");

            return true;
        }

        /// <summary>
        /// Deletes an object of type T and given identifier from Enova. Returns true if successfull.
        /// </summary>
        public bool Delete<T>(string identifier) where T : BaseObject
        {
            var context = _contextService.GetContext();
            var obj = context.FindObject<T>(identifier);

            if (obj == null)
                return false;

            _cacheService.ClearCache(obj.GetType());
            obj.Delete();

            _logger.LogInformation($"{_authService.LogUser()} deleted object with ID {obj.ID} and identifier {obj.Identifier} of type {obj.GetType().Name}");

            return true;
        }

        /// <summary>
        /// Adds a dynamic property to a type.
        /// </summary>
        public void AddProperty<T>(string propertyName, BaseObject.PropertyType propertyType, bool languageDependent, int maxStringLength)
        {
            var context = _contextService.GetContext();
            var derivedType = typeof(T).GetMostDerivedEnovaType();

            context.AddProperty(derivedType, propertyName, propertyType, maxStringLength, languageDependent);
        }

        /// <summary>
        /// Removes a dynamic property from a type.
        /// </summary>
        public void RemoveProperty<T>(string propertyName)
        {
            var context = _contextService.GetContext();
            var derivedType = typeof(T).GetMostDerivedEnovaType();

            //context.RemoveProperty(derivedType, propertyName);

            _mappingToEnovaService.ClearSettablePropertyCache(derivedType, propertyName);

            context.getalldynamicpropertynames?
        }



        private bool IsMemoryObject(Type derivedType)
        {
            var cmoClass = derivedType.GetCustomAttribute<CmoClassAttribute>()?.TryGetPropertyValue("CoreType",
                            BindingFlags.Instance |
                            BindingFlags.NonPublic |
                            BindingFlags.Public) as Type;
            var loadOnDemand = cmoClass?.GetCustomAttribute<LoadOnDemandAttribute>(inherit: true);

            return loadOnDemand == null;
        }

    }

}
