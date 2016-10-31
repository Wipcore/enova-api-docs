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
        object GetEnovaProperty(BaseObject obj, string propertyName);

        /// <summary>
        /// Map a property to given object.        
        /// </summary>
        void SetEnovaProperty(BaseObject obj, string propertyName, object value, IDictionary<string, object> otherValues);

        /// <summary>
        /// Higher number, higher priority for choosing which mapper to use.
        /// </summary>
        int Priority { get; } 

        /// <summary>
        /// Which sort of mapping this mapper handles.
        /// </summary>
        MapType MapType { get; }

        /// <summary>
        /// Set to true if this property should be mapped -after- the object is saved instead of before.
        /// </summary>
        bool PostSaveSet { get; }
    }

    public enum MapType
    {
        MapToEnovaAllowed, //mapping to enova, ie saving a property
        MapFromEnovaAllowed, //mapping from enova, ie querying
        MapFromAndToEnovaAllowed //map both from and to enova
    }
}
