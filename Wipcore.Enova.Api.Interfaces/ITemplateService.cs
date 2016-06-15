using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wipcore.Enova.Api.Models.Interfaces;

namespace Wipcore.Enova.Api.Interfaces
{
    public interface ITemplateService
    {
        /// <summary>
        /// Read template information from configuration - info of how queries should be processed, I.E default languages, properties etc.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="parameters">Parameters direct from query.</param>
        /// <returns></returns>
        IQueryModel GetQueryModelFromTemplateConfiguration(Type type, IQueryModel parameters);
    }
}
