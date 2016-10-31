﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Http;
using Fasterflect;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Generics;
using Wipcore.Enova.Api.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Wipcore.Core;
using Wipcore.Enova.Api.WebApi.Helpers;
using Wipcore.Enova.Api.Models;
using Wipcore.Enova.Api.Models.Interfaces;

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
        private readonly ILogger _logger;


        public ObjectService(IPagingService pagingService, ISortService sortService, IFilterService filterService, IMappingFromEnovaService mappingFromEnovaService,
            IMappingToEnovaService mappingToEnovaService, ITemplateService templateService, IContextService contextService, ILoggerFactory loggerFactory, IAuthService authService)
        {
            _pagingService = pagingService;
            _sortService = sortService;
            _filterService = filterService;
            _mappingFromEnovaService = mappingFromEnovaService;
            _mappingToEnovaService = mappingToEnovaService;
            _templateService = templateService;
            _contextService = contextService;
            _authService = authService;
            _logger = loggerFactory.CreateLogger(GetType().Name);
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
        /// Get objects from Enova. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestContext">Context for the query, ie language.</param>
        /// <param name="query">Query parameters.</param>
        /// <param name="candidates">Objects to look at, or null to look at all objects.</param>
        /// <returns></returns>
        public IEnumerable<IDictionary<string, object>> Get<T>(IContextModel requestContext, IQueryModel query, BaseObjectList candidates = null) where T : BaseObject
        {
            var derivedType = typeof(T).GetMostDerivedEnovaType();
            query = _templateService.GetQueryModelFromTemplateConfiguration(derivedType, query);
            
            var context = _contextService.GetContext();
            var memoryObject = IsMemoryObject(derivedType);

            //return from the candidates, or if memoryobject, get whats in memory. otherwise search the database
            var objectList = candidates ?? (memoryObject ? context.GetAllObjects(derivedType) :
                             context.Search(query.Filter ?? "ID > 0", derivedType, null, 0, null, false));

            objectList = _sortService.Sort(objectList, query.Sort);
            objectList = _filterService.Filter(objectList, query.Filter);
            objectList = _pagingService.Page(objectList, query.Page.Value, query.Size.Value);
            var objects = _mappingFromEnovaService.MapFromEnovaObject(objectList, query.Properties);

            return objects.ToList();
        }

        /// <summary>
        /// Save an object to Enova with the given values.
        /// </summary>
        /// <typeparam name="T">The most derived type of T is saved.</typeparam>
        /// <param name="requestContext">Context for the query, ie language.</param>
        /// <param name="values">Properties to save on the object.</param>
        /// <returns></returns>
        public IDictionary<string, object> Save<T>(IContextModel requestContext, Dictionary<string, object> values) where T : BaseObject
        {
            if (values == null)
                return null;

            var newObject = false;
            var context = _contextService.GetContext();

            var identifier = values.FirstOrDefault(x => x.Key.Equals("identifier", StringComparison.CurrentCultureIgnoreCase)).Value?.ToString();

            if(String.IsNullOrEmpty(identifier))
                throw new HttpException(HttpStatusCode.BadRequest, "Cannot save item without identifier.") ;
            
            var obj = context.FindObject<T>(identifier);

            if (obj == null)
            {
                obj = EnovaObjectCreationHelper.CreateNew<T>(context);
                newObject = true;
            }
            else
                obj.Edit();

            var postSaveMappers = _mappingToEnovaService.MapToEnovaObject(obj, values);
            obj.Save();
            
            postSaveMappers?.ForEach(x => x.Invoke());

            _logger.LogInformation("{0} {1} object with Identifier: {2} of Type: {3} with Values: {4}", _authService.LogUser(), newObject ? "Created" : "Updated", identifier, obj.GetType().Name, values.ToLog());

            var changedObject = context.FindObject<T>(obj.ID);//reget from enova to get any changes made
            return _mappingFromEnovaService.MapFromEnovaObject(changedObject, String.Join(",",values.Select(x => x.Key)));//remap to get changed values
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
