using System;

namespace Wipcore.Enova.Api.NETClient
{
    public class HttpClientSettings
    {
        public string Url { get; set; }

        public TimeSpan Timeout { get; set; }

        public HttpClientSettings()
        { }

        public HttpClientSettings(string url)
        {
            Url = url;
        }

        public HttpClientSettings(string url, TimeSpan timeout)
        {
            Url = url;
            Timeout = timeout;
        }
    }
}