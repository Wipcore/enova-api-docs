using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Wipcore.Enova.Api.Abstractions.Interfaces;
using Wipcore.Library;

namespace Wipcore.Enova.Api.Abstractions.Internal
{
    /// <summary>
    /// Base controller for the Enova Api. Derive from to create new controllers with similar behaviour as standard controllers. 
    /// </summary>
    public abstract class EnovaApiController : Controller
    {
        private readonly bool _autoLogControllerActions;
        protected static ILogger Logger = null;
        private readonly IExceptionService _exceptionService;
        private readonly IAuthService _authService;
        private readonly Stopwatch _stopwatch = new Stopwatch();

        protected EnovaApiController(EnovaApiControllerDependencies dependencies, bool autoLogControllerActions = true)
        {
            _autoLogControllerActions = autoLogControllerActions;
            _exceptionService = dependencies.ExceptionService;
            _authService = dependencies.AuthService;
            Logger = Logger ?? dependencies.LoggerFactory.CreateLogger(this.GetType());
            _stopwatch.Start();
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if(_autoLogControllerActions)
                Logger.LogTrace($"{_authService.LogUser()} executing {context.ActionDescriptor.DisplayName} with arguments ({String.Join(" / ", ArgumentsToString(context.ActionArguments))})");

            base.OnActionExecuting(context);
            
            //put any avaliable requestContext in cache for the request, so it can be retrived from anywhere
            context.ActionArguments.TryGetValue("requestContext", out var requestContext);

            if (requestContext is IContextModel)
                context.HttpContext.Items[WipConstants.ContextModelKey] = requestContext;

            
        }
        
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (!ModelState.IsValid)//output any model state errors as a bad request
            {
                var modelErrorMsgs = Environment.NewLine;
                if(ModelState.Root.Errors?.Any() == true)
                    modelErrorMsgs += String.Join(Environment.NewLine, ModelState.Root.Errors.Select(e => e.Exception?.Message ?? e.ErrorMessage));
                if(ModelState.Root.Children?.Any() == true)
                    modelErrorMsgs += String.Join(Environment.NewLine, ModelState.Root.Children.SelectMany(x => x.Errors.Select(e => e.Exception?.Message ?? e.ErrorMessage)));
                context.Exception = new HttpException(HttpStatusCode.BadRequest, modelErrorMsgs);
            }

            if (context.Exception != null)//handle any exception caused by action executed
            {
                _exceptionService.HandleControllerException(context);
            }

            base.OnActionExecuted(context);

            if(_autoLogControllerActions)
                Logger.LogTrace($"Finished executing {context.ActionDescriptor.DisplayName} in {_stopwatch.Elapsed}");
        }

        private IEnumerable<string> ArgumentsToString(IDictionary<string, object> arguments)
        {
            foreach (var argument in arguments)
            {
                if (argument.Value is IDictionary<string, object> subArguments)
                    yield return $"{argument.Key}: ({String.Join(", ", subArguments.Select(x => $"{x.Key}: {x.Value}"))})";
                else
                    yield return $"{argument.Key}: {argument.Value}";
            }
        }
    }
}
