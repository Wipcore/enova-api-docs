using Microsoft.Extensions.Logging;
using Wipcore.Enova.Api.Abstractions.Interfaces;

namespace Wipcore.Enova.Api.Abstractions
{
    public class EnovaApiControllerDependencies
    {
        public IExceptionService ExceptionService { get; }
        public ILoggerFactory LoggerFactory { get; }

        public EnovaApiControllerDependencies(IExceptionService exceptionService, ILoggerFactory loggerFactory)
        {
            ExceptionService = exceptionService;
            LoggerFactory = loggerFactory;
        }

    }
}
