using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Filters;
using Wipcore.Enova.Api.WebApi.Services;
using Wipcore.Enova.Api.Interfaces;

namespace Wipcore.Enova.Api.WebApi.Controllers
{
    public abstract class ApiController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            object requestContext;
            context.ActionArguments.TryGetValue("requestContext", out requestContext);

            if (requestContext is IContextModel)
                context.HttpContext.Items[ContextService.ContextModelKey] = requestContext;
        }
    }
}
