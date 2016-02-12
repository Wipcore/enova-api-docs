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

            //TODO limit size by configurable maxSize?
            int index = pageNumber > 0 ? (pageNumber - 1) * pageSize : 0;
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

            //TODO limit next to max page and prev to 0?

            string prevPage = UrlHelper.GetRequestUrl(httpContext).Replace("page=" + pageNumber.ToString(), "page=" + (pageNumber - 1).ToString());
            httpContext.Response.Headers.Add("X-Paging-PreviousPage", prevPage);
            string nextPage = UrlHelper.GetRequestUrl(httpContext).Replace("page=" + pageNumber.ToString(), "page=" + (pageNumber + 1).ToString());
            httpContext.Response.Headers.Add("X-Paging-NextPage", nextPage);
        }
    }
}
