using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Wipcore.eNova.Api.NETClient
{
    public class ProductClient
    {
        HttpClientWrapper _clientWrapper;

        public ProductClient(HttpClientSettings clientSettings)
        {
            _clientWrapper = new HttpClientWrapper(clientSettings);
        }

        public IDictionary<string, object> GetProduct(string id)
        {
            return _clientWrapper.Execute<IDictionary<string, object>>("api/products/" + id).Result;
        }

        public IEnumerable<IDictionary<string, object>> ListProducts(int pageSize, int page, out int pageCount, out string prevPageLink, out string nextPageLink)
        {
            var response = _clientWrapper.Execute("api/products?size=" + pageSize.ToString() + "&page=" + page.ToString());
            string json = response.Result.Content.ReadAsStringAsync().Result;

            string pagesCount = response.Result.Headers.GetValues("X-Paging-PageCount").FirstOrDefault();
            Int32.TryParse(pagesCount, out pageCount);
            string currentPageSize = response.Result.Headers.GetValues("X-Paging-PageSize").FirstOrDefault();
            string objCount = response.Result.Headers.GetValues("X-Paging-TotalRecordCount").FirstOrDefault();
            string currentPage = response.Result.Headers.GetValues("X-Paging-PageNo").FirstOrDefault();
            prevPageLink = response.Result.Headers.GetValues("X-Paging-PreviousPage").FirstOrDefault();
            nextPageLink = response.Result.Headers.GetValues("X-Paging-NextPage").FirstOrDefault();

            return JsonConvert.DeserializeObject<IEnumerable<IDictionary<string, object>>>(json);
        }
    }
}
