using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Wipcore.Enova.Api.Abstractions.Interfaces;

namespace Wipcore.Enova.Api.WebApi.Controllers
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
