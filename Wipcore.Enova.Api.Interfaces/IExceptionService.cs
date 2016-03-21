using System;
using System.Net;
using Microsoft.AspNet.Mvc.Filters;

namespace Wipcore.Enova.Api.Interfaces
{
    public interface IExceptionService
    {
        void HandleControllerException(ActionExecutedContext context);

        HttpStatusCode GetStatusCodeForException(Exception exception);
    }
}