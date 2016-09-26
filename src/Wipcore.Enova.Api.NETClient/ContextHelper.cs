using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wipcore.Enova.Api.Models;
using System.Reflection;

namespace Wipcore.Enova.Api.NetClient
{
    public static class ContextHelper
    {
        public static string GetContextParameters(ContextModel context)
        {
            if (context == null)
                return String.Empty;

            var sb = new StringBuilder();
            sb.Append("&");
            foreach (var p in typeof(ContextModel).GetProperties())
            {
                var value = p.GetValue(context);
                if (value == null)
                    continue;

                sb.Append(p.Name);
                sb.Append("=");
                sb.Append(value);
                sb.Append("&");
            }
            sb.Remove(sb.Length - 1, 1);

            return sb.ToString();
        }
    }
}
