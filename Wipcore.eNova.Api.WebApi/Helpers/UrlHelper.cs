using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;

namespace Wipcore.Enova.Api.WebApi.Helpers
{
    public static class UrlHelper
    {
        /// <summary>
        /// Get the url of the current request.
        /// </summary>
        public static string GetRequestUrl(HttpContext context)
        {
            var absoluteUri = String.Concat(
                        context.Request.Scheme,
                        "://",
                        context.Request.Host.ToUriComponent(),
                        context.Request.PathBase.ToUriComponent(),
                        context.Request.Path.ToUriComponent(),
                        context.Request.QueryString.ToUriComponent());
            return absoluteUri;
        }

        /// <summary>
        /// True if the request has parameters in query string.
        /// </summary>
        public static bool RequestHasParameters(HttpContext context)
        {
            return context.Request.QueryString.HasValue;
        }
    }
}
