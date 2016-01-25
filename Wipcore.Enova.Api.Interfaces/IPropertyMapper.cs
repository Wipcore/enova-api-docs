using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wipcore.Core.SessionObjects;

namespace Wipcore.eNova.Api.Interfaces
{
    public interface IPropertyMapper
    {
        string Name { get; }

        Type Type { get; }

        bool InheritMapper { get; } //true if a derived class can use this mapper to map its property. Usually true.

        object Map(BaseObject obj);

        int Priority { get; } //higher number, higher priority
    }
}
