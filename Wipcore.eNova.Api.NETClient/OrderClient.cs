using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Wipcore.Enova.Api.Models;

namespace Wipcore.Enova.Api.NetClient
{
    public class OrderClient
    {
        private readonly HttpClientWrapper _clientWrapper;

        public OrderClient(HttpClientSettings clientSettings)
        {
            _clientWrapper = new HttpClientWrapper(clientSettings);
        }

        public async Task<ResponseModel> GetOrder(string identifier, ContextModel context = null)
        {
            var response = await _clientWrapper.Get("api/orders/" + identifier + ContextHelper.GetContextParameters(context));
            var json = await response.Content.ReadAsStringAsync();

            var model = new ResponseModel();
            model.StatusCode = response.StatusCode;
            if (response.IsSuccessStatusCode)
            {
                // TODO: change to OrderModel?
                //model.Object = JsonConvert.DeserializeObject<CartModel>(json);
                model.Object = JsonConvert.DeserializeObject<IDictionary<string, object>>(json);
            }

            return model;
        }

        public async Task<ResponseModel> ListOrders(int pageSize, int page, string sort = "", string filter = "", string properties = "", ContextModel context = null)
        {
            string optionalParams = String.Empty;
            if (!String.IsNullOrEmpty(sort))
                optionalParams += "&sort=" + sort;
            if (!String.IsNullOrEmpty(filter))
                optionalParams += "&filter=" + filter;
            if (!String.IsNullOrEmpty(properties))
                optionalParams += "&properties=" + properties;

            var response = await _clientWrapper.Get("api/orders?size=" + pageSize.ToString() + "&page=" + page.ToString() + optionalParams + ContextHelper.GetContextParameters(context));
            var json = response.Content.ReadAsStringAsync();

            var model = new ResponseModel();
            model.StatusCode = response.StatusCode;
            if (response.IsSuccessStatusCode)
            {
                var list = new ListModel();

                // TODO: change to OrderModel?
                list.Objects = JsonConvert.DeserializeObject<IEnumerable<IDictionary<string, object>>>(json.Result);

                int pageCount, recordCount;
                string pagesCount = response.Headers.GetValues("X-Paging-PageCount").FirstOrDefault();
                Int32.TryParse(pagesCount, out pageCount);
                string recordsCount = response.Headers.GetValues("X-Paging-TotalRecordCount").FirstOrDefault();
                Int32.TryParse(recordsCount, out recordCount);

                list.PageCount = pageCount;
                list.RecordCount = recordCount;
                list.PageSize = pageSize;
                list.CurrentPageIndex = page;
                list.PreviousPage = response.Headers.GetValues("X-Paging-PreviousPage").FirstOrDefault();
                list.NextPage = response.Headers.GetValues("X-Paging-NextPage").FirstOrDefault();

                model.List = list;
            }

            return model;
        }

        public async Task<ResponseModel> CreateOrder(CartModel cart, ContextModel context = null)
        {
            // TODO: better errorhandling
            var response = await _clientWrapper.Post("api/orders?" + ContextHelper.GetContextParameters(context), JsonConvert.SerializeObject(cart));
            var json = await response.Content.ReadAsStringAsync();

            var model = new ResponseModel();
            model.StatusCode = response.StatusCode;
            if (response.IsSuccessStatusCode)
            {
                // TODO: change to OrderModel?
                model.Object = JsonConvert.DeserializeObject<CartModel>(json);
            }

            return model;
        }
    }
}
