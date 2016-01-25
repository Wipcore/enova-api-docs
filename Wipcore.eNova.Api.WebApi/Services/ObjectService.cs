using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Generics;
using Wipcore.eNova.Api.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNet.Http;

namespace Wipcore.eNova.Api.WebApi.Services
{
    public class ObjectService : IObjectService
    {
        private readonly IConfigurationRoot _configuration;
        private readonly IPagingService _pagingService;
        private readonly ISortService _sortService;
        private readonly IFilterService _filterService;
        private readonly IMappingService _mappingService;
        

        public ObjectService(IConfigurationRoot configuration, IPagingService pagingService, ISortService sortService, 
            IFilterService filterService, IMappingService mappingService)
        {
            _configuration = configuration;
            _pagingService = pagingService;
            _sortService = sortService;
            _filterService = filterService;
            _mappingService = mappingService;
        }

        public IDictionary<string, object> Get<T>(string identifier, string properties) where T : BaseObject
        {
            var context = EnovaContextProvider.GetCurrentContext();
            var obj = context.FindObject<T>(identifier);

            return _mappingService.GetProperties(obj, properties);
        }

        public IEnumerable<IDictionary<string, object>> Get<T>(int pageNumber, int pageSize, string properties, string sort, string filter) where T : BaseObject
        {
            var context = EnovaContextProvider.GetCurrentContext();
            var objectList = context.GetAllObjects(typeof(T)); //TODO as usual propblem with items not in memory
            
            objectList = _sortService.Sort(objectList, sort);
            objectList = _filterService.Filter(objectList, filter);
            objectList = _pagingService.Page(objectList, pageNumber, pageSize);
            var objects = _mappingService.GetProperties(objectList, properties);

            return objects;
        }

        

        
        

        

        
    }

    
}
