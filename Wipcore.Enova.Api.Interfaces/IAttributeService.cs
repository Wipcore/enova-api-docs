using Wipcore.Core.SessionObjects;

namespace Wipcore.Enova.Api.Interfaces
{
    public interface IAttributeService
    {
        BaseObjectList GetAttributes<T>(string identifier) where T : BaseObject;
    }
}