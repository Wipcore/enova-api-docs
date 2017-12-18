﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Wipcore.Enova.Api.Abstractions.Interfaces;

namespace Wipcore.Enova.Api.Abstractions
{
    /// <summary>
    /// Base controller for the Enova Api. Derive from to create new controllers with similar behaviour as standard controllers. 
    /// </summary>
    public abstract class EnovaApiController : Controller
    {

        private readonly IExceptionService _exceptionService;
        private readonly ILogger _logger;

        protected EnovaApiController(EnovaApiControllerDependencies dependencies)
        {
            _exceptionService = dependencies.ExceptionService;
            _logger = dependencies.LoggerFactory.CreateLogger(this.GetType());
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            //put any avaliable requestContext in cache for the request, so it can be retrived from anywhere
            context.ActionArguments.TryGetValue("requestContext", out var requestContext);

            if (requestContext is IContextModel)
                context.HttpContext.Items[WipConstants.ContextModelKey] = requestContext;
        }
        
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception != null)//handle any exception caused by action executed
            {
                _exceptionService.HandleControllerException(context);
            }

            base.OnActionExecuted(context);
        }
    }
}
