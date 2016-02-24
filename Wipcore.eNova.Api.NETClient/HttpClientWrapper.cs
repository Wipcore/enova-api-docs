using System;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;

namespace Wipcore.Enova.Api.NetClient
{
    public class HttpClientWrapper
    {
        private HttpClientSettings _settings;
        private HttpClient _client;

        public HttpClientWrapper(HttpClientSettings settings)
        {
            _settings = settings;
            _client = new HttpClient();
            _client.BaseAddress = new Uri(_settings.Url);
            if (_settings.Timeout != TimeSpan.Zero)
                _client.Timeout = _settings.Timeout;
        }

        public async Task<T> Get<T>(string route)
        {
            var json = await _client.GetStringAsync(_settings.Url + '/' + route.TrimStart('/'));
            return JsonConvert.DeserializeObject<T>(json);
        }

        public async Task<HttpResponseMessage> Get(string route)
        {
            return await _client.GetAsync(_settings.Url + '/' + route.TrimStart('/'));
        }

        public async Task<HttpResponseMessage> Post(string route, string content)
        {
            var stringContent = new System.Net.Http.StringContent(content, new UTF8Encoding(), "application/json");
            return await _client.PostAsync(_settings.Url + '/' + route.TrimStart('/'), stringContent);
        }

        public async Task<HttpResponseMessage> Put(string route, string content)
        {
            var stringContent = new System.Net.Http.StringContent(content, new UTF8Encoding(), "application/json");
            return await _client.PutAsync(_settings.Url + '/' + route.TrimStart('/'), stringContent);
        }
    }
}
