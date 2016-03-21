using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using Wipcore.eNova.Api.WebApi.Helpers;
using Wipcore.eNova.Api.WebApi.Services;
using Wipcore.Enova.Api.WebApi.Services;
using Wipcore.Enova.Api.Interfaces;
using Wipcore.Enova.Api.Models.Interfaces;

namespace Wipcore.Enova.Api.WebApi.Controllers
{
    public abstract class ApiController : Controller
    {

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            //put any avaliable requestContext in cache for the request, so it can be retrived from anywhere
            object requestContext;
            context.ActionArguments.TryGetValue("requestContext", out requestContext);

            if (requestContext is IContextModel)
                context.HttpContext.Items[ContextService.ContextModelKey] = requestContext;
        }
        
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception != null)
            {
                var exceptionService = this.Resolver.GetService<IExceptionService>();
                exceptionService.HandleControllerException(context);
            }

            base.OnActionExecuted(context);
        }
    }
}
