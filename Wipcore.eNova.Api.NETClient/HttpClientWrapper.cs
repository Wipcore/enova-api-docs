using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;

namespace Wipcore.Enova.Api.NETClient
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

        public async Task<T> Execute<T>(string route)
        {
            var json = await _client.GetStringAsync(_settings.Url + '/' + route.TrimStart('/'));
            return JsonConvert.DeserializeObject<T>(json);
        }

        public async Task<HttpResponseMessage> Execute(string route)
        {
            return await _client.GetAsync(_settings.Url + '/' + route.TrimStart('/'));
        }
    }
}
