using Wipcore.Core.SessionObjects;

namespace Wipcore.Enova.Api.Interfaces
{
    public interface IWarehoseService
    {
        BaseObjectList GetWarehoseCompartments(string productIdentifier, string warehouseIdentifier = null);
    }
}