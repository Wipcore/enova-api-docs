using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wipcore.Enova.Api.NETClient
{
    public class ListModel
    {
        public IEnumerable<IDictionary<string, object>> Objects { get; set; }

        public int PageCount { get; set; }

        public int PageSize { get; set; }

        public int RecordCount { get; set; }

        public int CurrentPageIndex { get; set; }

        public string PreviousPage { get; set; }

        public string NextPage { get; set; }
    }
}
