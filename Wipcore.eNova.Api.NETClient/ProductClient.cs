using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Wipcore.Enova.Api.NetClient
{
    public class ProductClient
    {
        HttpClientWrapper _clientWrapper;

        public ProductClient(HttpClientSettings clientSettings)
        {
            _clientWrapper = new HttpClientWrapper(clientSettings);
        }

        public async Task<IDictionary<string, object>> GetProduct(string id)
        {
            return await _clientWrapper.Get<IDictionary<string, object>>("api/products/" + id);
        }

        public async Task<ListModel> ListProducts(int pageSize, int page)
        {
            var response = await _clientWrapper.Get("api/products?size=" + pageSize.ToString() + "&page=" + page.ToString());
            var json = response.Content.ReadAsStringAsync();

            var model = new ListModel();

            model.Objects = JsonConvert.DeserializeObject<IEnumerable<IDictionary<string, object>>>(json.Result);

            int pageCount, recordCount;
            string pagesCount = response.Headers.GetValues("X-Paging-PageCount").FirstOrDefault();
            Int32.TryParse(pagesCount, out pageCount);
            string recordsCount = response.Headers.GetValues("X-Paging-TotalRecordCount").FirstOrDefault();
            Int32.TryParse(recordsCount, out recordCount);

            model.PageCount = pageCount;
            model.RecordCount = recordCount;
            model.PageSize = pageSize;
            model.CurrentPageIndex = page;
            model.PreviousPage = response.Headers.GetValues("X-Paging-PreviousPage").FirstOrDefault();
            model.NextPage = response.Headers.GetValues("X-Paging-NextPage").FirstOrDefault();

            return model;
        }
    }
}
