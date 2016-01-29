using System;
using System.Collections.Generic;
using System.Linq;
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
using Wipcore.eNova.Api.WebApi.Models;

namespace Wipcore.eNova.Api.WebApi.Services
{
    public class ObjectService : IObjectService
    {
        private readonly IConfigurationRoot _configuration;
        private readonly IPagingService _pagingService;
        private readonly ISortService _sortService;
        private readonly IFilterService _filterService;
        private readonly IMappingService _mappingService;
        private readonly ILocationService _locationService;
        private readonly IContextService _contextService;


        public ObjectService(IConfigurationRoot configuration, IPagingService pagingService, ISortService sortService, 
            IFilterService filterService, IMappingService mappingService, ILocationService locationService, IContextService contextService)
        {
            _configuration = configuration;
            _pagingService = pagingService;
            _sortService = sortService;
            _filterService = filterService;
            _mappingService = mappingService;
            _locationService = locationService;
            _contextService = contextService;
        }

        public IDictionary<string, object> Get<T>(string identifier, string properties) where T : BaseObject
        {
            var context = EnovaContextProvider.GetCurrentContext();//TODO config here too?
            var obj = context.FindObject<T>(identifier);

            return _mappingService.GetProperties(obj, properties);
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
            var objects = _mappingService.GetProperties(objectList, getParameters.Properties);

            return objects;
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
