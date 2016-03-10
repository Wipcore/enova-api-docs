using System;
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
using Microsoft.AspNet.Http;
using Wipcore.Core;
using Wipcore.Enova.Api.WebApi.Helpers;
using Wipcore.Enova.Api.Models;
using Wipcore.Enova.Api.Models.Interfaces;

namespace Wipcore.Enova.Api.WebApi.Services
{
    public class ObjectService : IObjectService
    {
        private readonly IPagingService _pagingService;
        private readonly ISortService _sortService;
        private readonly IFilterService _filterService;
        private readonly IMappingFromService _mappingFromService;
        private readonly IMappingToService _mappingToService;
        private readonly ILocationService _locationService;
        private readonly IContextService _contextService;


        public ObjectService(IPagingService pagingService, ISortService sortService, IFilterService filterService, IMappingFromService mappingFromService,
            IMappingToService mappingToService, ILocationService locationService, IContextService contextService)
        {
            _pagingService = pagingService;
            _sortService = sortService;
            _filterService = filterService;
            _mappingFromService = mappingFromService;
            _mappingToService = mappingToService;
            _locationService = locationService;
            _contextService = contextService;
        }

        public IDictionary<string, object> Get<T>(IContextModel requestContext, IGetParametersModel getParameters, string identifier) where T : BaseObject
        {
            var context = _contextService.GetContext();
            getParameters = _locationService.GetParametersFromLocationConfiguration(typeof(T).Name, getParameters);

            var obj = context.FindObject<T>(identifier);
            return _mappingFromService.MapFrom(obj, getParameters.Properties);
        }

        public IEnumerable<IDictionary<string, object>> Get<T>(IContextModel requestContext, IGetParametersModel getParameters) where T : BaseObject
        {
            getParameters = _locationService.GetParametersFromLocationConfiguration(typeof(T).Name, getParameters);

            var context = _contextService.GetContext();
            var memoryObject = IsMemoryObject<T>();

            //if memoryobject, get whats in memory. otherwise search the database
            var objectList = memoryObject ? context.GetAllObjects(typeof(T)) :
                             context.Search(getParameters.Filter ?? "ID > 0", typeof(T), null, 0, null, false);

            objectList = _sortService.Sort(objectList, getParameters.Sort);
            objectList = _filterService.Filter(objectList, getParameters.Filter);
            objectList = _pagingService.Page(objectList, getParameters.Page.Value, getParameters.Size.Value);
            var objects = _mappingFromService.MapFrom(objectList, getParameters.Properties);

            return objects;
        }

        public IDictionary<string, object> Save<T>(IContextModel requestContext, Dictionary<string, object> values) where T : BaseObject
        {
            var context = _contextService.GetContext();
            T obj = null;

            //find object by id or identifier, and create new if nothing is found
            object idValue;
            values.TryGetValue("id", out idValue);

            if (idValue != null)
            {
                var id = Convert.ToInt32(idValue);
                obj = context.FindObject<T>(id);
                if(obj == null)
                    throw new HttpResponseException(new HttpResponseMessage() {StatusCode = HttpStatusCode.NotFound,
                        ReasonPhrase = String.Format("Object with id {0} does not exist to be updated.", id)});
            }
            else
            {
                object identifier;
                values.TryGetValue("identifier", out identifier);

                if(identifier != null)
                    obj = context.FindObject<T>(identifier.ToString());
            }

            if (obj == null)
                obj = EnovaObjectCreationHelper.CreateNew<T>(context);
            else
                obj.Edit();

            var resultingObject = _mappingToService.MapTo(obj, values);
            obj.Save();

            return resultingObject;
        }

        private bool IsMemoryObject<T>()
        {
            var cmoClass = typeof(T).GetCustomAttribute<CmoClassAttribute>()?.TryGetPropertyValue("CoreType", 
                            BindingFlags.Instance |
                            BindingFlags.NonPublic |
                            BindingFlags.Public) as Type;
            var loadOnDemand = cmoClass?.GetCustomAttribute<LoadOnDemandAttribute>(inherit: true);

            return loadOnDemand == null;
        }
        
    }
    
}
