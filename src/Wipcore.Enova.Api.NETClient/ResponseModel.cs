using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wipcore.Enova.Api.NetClient
{
    public class ResponseModel
    {
        public System.Net.HttpStatusCode StatusCode { get; set; }
        public string ResponseMessage { get; set; }
        public ListModel List { get; set; }
        public object Object { get; set; }
    }
}
