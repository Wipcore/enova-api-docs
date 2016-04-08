using System;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Text;

namespace Wipcore.Enova.Api.NetClient
{
    public class HttpClientWrapper
    {
        private readonly HttpClientSettings _settings;
        private readonly HttpClient _client;

        public HttpClientWrapper(HttpClientSettings settings)
        {
            _settings = settings;
            _client = new HttpClient {BaseAddress = new Uri(_settings.Url)};
            if (_settings.Timeout != TimeSpan.Zero)
                _client.Timeout = _settings.Timeout;
        }

        public T Get<T>(string route)
        {
            var json = _client.GetStringAsync(_settings.Url.TrimEnd('/') + '/' + route.TrimStart('/')).Result;
            return JsonConvert.DeserializeObject<T>(json);
        }

        public HttpResponseMessage Get(string route)
        {
            return _client.GetAsync(_settings.Url.TrimEnd('/') + '/' + route.TrimStart('/')).Result;
        }

        public HttpResponseMessage Post(string route, string content)
        {
            var stringContent = new StringContent(content, new UTF8Encoding(), "application/json");
            return _client.PostAsync(_settings.Url.TrimEnd('/') + '/' + route.TrimStart('/'), stringContent).Result;
        }

        public HttpResponseMessage Put(string route, string content)
        {
            var stringContent = new StringContent(content, new UTF8Encoding(), "application/json");
            return _client.PutAsync(_settings.Url.TrimEnd('/') + '/' + route.TrimStart('/'), stringContent).Result;
        }
    }
}
