using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wipcore.Core.SessionObjects;

namespace Wipcore.Enova.Api.Interfaces
{
    /// <summary>
    /// Implement to provide logic for mapping a property.
    /// </summary>
    public interface IPropertyMapper
    {
        /// <summary>
        /// Names of the properties handled by this mapper.
        /// </summary>
        List<string> Names { get; }

        /// <summary>
        /// The type of object that this map handles.
        /// </summary>
        Type Type { get; }

        /// <summary>
        /// True if a derived class can use this mapper to map its property. Usually true.
        /// </summary>
        bool InheritMapper { get; } 

        /// <summary>
        /// Map a property from given object.
        /// </summary>
        object MapFromEnovaProperty(BaseObject obj, string propertyName);

        /// <summary>
        /// Map a property to given object.        
        /// </summary>
        object MapToEnovaProperty(BaseObject obj, string propertyName, object value);

        /// <summary>
        /// Higher number, higher priority for choosing which mapper to use.
        /// </summary>
        int Priority { get; } 

        /// <summary>
        /// Which sort of mapping this mapper handles.
        /// </summary>
        MapType MapType { get; }
    }

    public enum MapType
    {
        MapTo, //mapping to enova, ie saving a property
        MapFrom, //mapping from enova, ie querying
        MapAll //map both from and to enova
    }
}
