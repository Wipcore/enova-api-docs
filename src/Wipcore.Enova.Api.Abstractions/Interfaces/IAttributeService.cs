using Wipcore.Core.SessionObjects;

namespace Wipcore.Enova.Api.Abstractions.Interfaces
{
    public interface IAttributeService
    {
        /// <summary>
        /// Get attributes of the object with the given identifier.
        /// </summary>
        BaseObjectList GetAttributes<T>(string identifier) where T : BaseObject;
    }
}