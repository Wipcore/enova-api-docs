using System;
using System.Net;
using Microsoft.AspNet.Mvc.Filters;

namespace Wipcore.Enova.Api.Interfaces
{
    public interface IExceptionService
    {
        /// <summary>
        /// Handles a controller exception by setting response code, message, and logging it.
        /// </summary>
        /// <param name="context"></param>
        void HandleControllerException(ActionExecutedContext context);

        /// <summary>
        /// Get an appropriate status code for the given exception. (I.E 404 for not found object).
        /// </summary>
        HttpStatusCode GetStatusCodeForException(Exception exception);
    }
}