using System;
using Wipcore.Enova.Api.Abstractions.Interfaces;

namespace Wipcore.Enova.Api.Abstractions.Internal
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
