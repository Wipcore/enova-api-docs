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

            StringBuilder sb = new StringBuilder();
            sb.Append("&");
            foreach (var p in typeof(ContextModel).GetFields())
            {
                sb.Append(p.Name);
                sb.Append("=");
                sb.Append(p.GetValue(context).ToString());
                sb.Append("&");
            }
            sb.Remove(sb.Length, 1);

            return sb.ToString();
        }
    }
}
