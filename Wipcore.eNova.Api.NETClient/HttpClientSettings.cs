using System;

namespace Wipcore.eNova.Api.NETClient
{
    public class HttpClientSettings
    {
        public string Url { get; set; }
        public TimeSpan Timeout { get; set; }
    }
}