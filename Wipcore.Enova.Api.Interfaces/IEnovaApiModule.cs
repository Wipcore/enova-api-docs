using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Core;

namespace Wipcore.Enova.Api.Interfaces
{
    public interface IEnovaApiModule : IModule
    {
        int Priority { get; }
    }
}
