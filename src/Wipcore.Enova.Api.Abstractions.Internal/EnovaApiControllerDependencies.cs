using Microsoft.Extensions.Logging;
using Wipcore.Enova.Api.Abstractions.Interfaces;

namespace Wipcore.Enova.Api.Abstractions.Internal
{
    public class EnovaApiControllerDependencies
    {
        public IExceptionService ExceptionService { get; }
        public ILoggerFactory LoggerFactory { get; }
        public IObjectService ObjectService { get; }
        public IAuthService AuthService { get; }

        public EnovaApiControllerDependencies(IExceptionService exceptionService, ILoggerFactory loggerFactory, IObjectService objectService, IAuthService authService)
        {
            ExceptionService = exceptionService;
            LoggerFactory = loggerFactory;
            ObjectService = objectService;
            AuthService = authService;
        }

    }
}
