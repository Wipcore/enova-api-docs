using System;
using System.Net;

namespace Wipcore.Enova.Api.Abstractions.Internal
{
    /// <summary>
    /// A HttpException with statuscode and message.
    /// </summary>
    public class HttpException : Exception
    {
        public HttpStatusCode StatusCode { get; }

        public HttpException(HttpStatusCode statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
