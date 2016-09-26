using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Wipcore.Enova.Api.Models;
using static System.String;

namespace Wipcore.Enova.Api.NetClient
{
    public class ProductClient
    {
        private readonly HttpClientWrapper _clientWrapper;

        public ProductClient(HttpClientSettings clientSettings)
        {
            _clientWrapper = new HttpClientWrapper(clientSettings);
        }

        public ResponseModel GetProduct(string identifier, ContextModel context = null)
        {
            var response = _clientWrapper.Get("api/products/" + identifier + ContextHelper.GetContextParameters(context));
            var json = response.Content.ReadAsStringAsync().Result;

            var model = new ResponseModel {StatusCode = response.StatusCode};
            if (response.IsSuccessStatusCode)
            {
                model.Object = JsonConvert.DeserializeObject<IDictionary<string, object>>(json);
            }

            return model;
        }

        public ResponseModel ListProducts(int pageSize, int page, string sort = "", string filter = "", string properties = "", ContextModel context = null)
        {
            var optionalParams = Empty;
            if (!IsNullOrEmpty(sort))
                optionalParams += "&sort=" + sort;
            if (!IsNullOrEmpty(filter))
                optionalParams += "&filter=" + filter;
            if (!IsNullOrEmpty(properties))
                optionalParams += "&properties=" + properties;

            var response = _clientWrapper.Get("api/products?size=" + pageSize + "&page=" + page + optionalParams + ContextHelper.GetContextParameters(context));
            var json = response.Content.ReadAsStringAsync().Result;

            var model = new ResponseModel {StatusCode = response.StatusCode};
            if (!response.IsSuccessStatusCode)
                return model;

            var list = new ListModel
            {
                Objects = JsonConvert.DeserializeObject<IEnumerable<IDictionary<string, object>>>(json)
            };


            int pageCount, recordCount;
            var pagesCount = response.Headers.GetValues("X-Paging-PageCount").FirstOrDefault();
            Int32.TryParse(pagesCount, out pageCount);
            var recordsCount = response.Headers.GetValues("X-Paging-TotalRecordCount").FirstOrDefault();
            Int32.TryParse(recordsCount, out recordCount);

            list.PageCount = pageCount;
            list.RecordCount = recordCount;
            list.PageSize = pageSize;
            list.CurrentPageIndex = page;
            list.PreviousPage = response.Headers.GetValues("X-Paging-PreviousPage").FirstOrDefault();
            list.NextPage = response.Headers.GetValues("X-Paging-NextPage").FirstOrDefault();

            model.List = list;

            return model;
        }

        public ResponseModel GetPrice(string identifier, ContextModel context = null)
        {
            var response = this.GetPrices(new[] { identifier }, 1, 1, context);

            if (response.List?.Objects == null)
                return response;

            response.Object = response.List.Objects.FirstOrDefault();
            response.List = null;

            return response;
        }

        public ResponseModel GetPrices(IEnumerable<string> identifiers, int pageSize, int page, ContextModel context = null)
        {
            const string template = "price";
            var filter = Join(" or ", identifiers.Where(x => !IsNullOrEmpty(x)).Select(x => "identifier =" + x));

            var response = _clientWrapper.Get("api/products?filter=" + filter + "&template=" + template + "&size=" + pageSize +
                " &page = " + page + ContextHelper.GetContextParameters(context));
            var json = response.Content.ReadAsStringAsync().Result;

            var model = new ResponseModel { StatusCode = response.StatusCode };
            if (!response.IsSuccessStatusCode)
                return model;

            int pageCount, recordCount;
            var pagesCount = response.Headers.GetValues("X-Paging-PageCount").FirstOrDefault();
            Int32.TryParse(pagesCount, out pageCount);
            var recordsCount = response.Headers.GetValues("X-Paging-TotalRecordCount").FirstOrDefault();
            Int32.TryParse(recordsCount, out recordCount);

            model.List = new ListModel
            {
                Objects = JsonConvert.DeserializeObject<IEnumerable<IDictionary<string, object>>>(json),
                PageCount = pageCount,
                RecordCount = recordCount,
                PageSize = pageSize,
                CurrentPageIndex = page,
                PreviousPage = response.Headers.GetValues("X-Paging-PreviousPage").FirstOrDefault(),
                NextPage = response.Headers.GetValues("X-Paging-NextPage").FirstOrDefault()
            };

            return model;
        }
    }
}
