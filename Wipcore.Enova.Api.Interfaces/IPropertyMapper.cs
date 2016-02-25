using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wipcore.Core.SessionObjects;

namespace Wipcore.Enova.Api.Interfaces
{
    public interface IPropertyMapper
    {
        List<string> Names { get; }

        Type Type { get; }

        bool InheritMapper { get; } //true if a derived class can use this mapper to map its property. Usually true.

        object MapFrom(BaseObject obj, string propertyName);

        object MapTo(BaseObject obj, string propertyName);

        int Priority { get; } //higher number, higher priority

        MapType MapType { get; }
    }

    public enum MapType
    {
        MapTo,
        MapFrom,
        MapAll
    }
}
