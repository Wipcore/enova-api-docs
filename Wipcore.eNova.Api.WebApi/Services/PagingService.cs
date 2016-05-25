using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Generics;
using Wipcore.Enova.Api.Interfaces;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using System.Web;
using Wipcore.Enova.Api.WebApi.Helpers;

namespace Wipcore.Enova.Api.WebApi.Services
{
    public class PagingService : IPagingService
    {
        private readonly IHttpContextAccessor _httpAccessor;

        public PagingService(IHttpContextAccessor httpAccessor)
        {
            _httpAccessor = httpAccessor;
        }

        public BaseObjectList Page(BaseObjectList objects, int pageNumber, int pageSize)
        {
            this.OutputPagingHeaders(pageNumber, pageSize, objects.Count);

            var index = pageNumber > 0 ? (pageNumber - 1) * pageSize : 0;
            objects = objects.GetRange(index, pageSize);
            return objects;
        }

        private void OutputPagingHeaders(int pageNumber, int pageSize, int objectCount)
        {
            int pageCount = (objectCount + pageSize - 1) / pageSize;

            var httpContext = _httpAccessor.HttpContext;
            httpContext.Response.Headers.Add("X-Paging-PageNo", pageNumber.ToString());
            httpContext.Response.Headers.Add("X-Paging-PageSize", pageSize.ToString());
            httpContext.Response.Headers.Add("X-Paging-PageCount", pageCount.ToString());
            httpContext.Response.Headers.Add("X-Paging-TotalRecordCount", objectCount.ToString());

            var nextPage = Math.Min(pageNumber + 1, pageCount);
            var previousPage = Math.Max(pageNumber - 1, 1);
            
            string prevPageLink;
            string nextPageLink;

            var requestUrl = UrlHelper.GetRequestUrl(httpContext);
            if (requestUrl.Contains("page=" + pageNumber))
            {
                prevPageLink = requestUrl.Replace("page=" + pageNumber, "page=" + previousPage);
                nextPageLink = requestUrl.Replace("page=" + pageNumber, "page=" + nextPage);
            }
            else if (requestUrl.Contains("Page=" + pageNumber))
            {
                prevPageLink = requestUrl.Replace("Page=" + pageNumber, "Page=" + previousPage);
                nextPageLink = requestUrl.Replace("Page=" + pageNumber, "Page=" + nextPage);
            }
            else
            {
                var prefix = UrlHelper.RequestHasParameters(httpContext) ? "&" : "?";
                prevPageLink = requestUrl + prefix + "Page=" + previousPage;
                nextPageLink = requestUrl + prefix + "Page=" + nextPage;
            }

            httpContext.Response.Headers.Add("X-Paging-PreviousPage", prevPageLink);
            httpContext.Response.Headers.Add("X-Paging-NextPage", nextPageLink);
        }
    }
}
