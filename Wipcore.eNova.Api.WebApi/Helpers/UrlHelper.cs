using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;

namespace Wipcore.eNova.Api.WebApi.Helpers
{
    public static class UrlHelper
    {
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
    }
}
