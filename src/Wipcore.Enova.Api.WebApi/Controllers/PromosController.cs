using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Wipcore.Enova.Api.WebApi.Services;
using Wipcore.Enova.Core;
using Wipcore.Enova.Api.Interfaces;
using System.Web.Http;
using Microsoft.AspNetCore.Authorization;
using Wipcore.Enova.Api.Models;
using Wipcore.Enova.Api.OAuth;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Wipcore.Enova.Api.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class PromosController : EnovaApiController
    {
        private readonly IObjectService _objectService;
       

        public PromosController(IExceptionService exceptionService,IObjectService objectService)
            : base(exceptionService)
        {
            _objectService = objectService;
        }

        /// <summary>
        /// Get a list of promos.
        /// </summary>
        [HttpGet()]
        [Authorize(Roles = AuthService.AdminRole)]
        public IEnumerable<IDictionary<string, object>> Get([FromUri] ContextModel requestContext, [FromUri] QueryModel query)
        {
            return _objectService.Get<EnovaPromo>(requestContext, query);
        }

        /// <summary>
        /// Get a promo specified by identifier. 
        /// </summary>
        [HttpGet("{identifier}")]
        public IDictionary<string, object> Get(ContextModel requestContext, QueryModel query, string identifier)
        {
            return _objectService.Get<EnovaPromo>(requestContext, query, identifier);
        }
    }
}
