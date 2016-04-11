using Wipcore.Core.SessionObjects;

namespace Wipcore.Enova.Api.Interfaces
{
    public interface IWarehouseService
    {
        BaseObjectList GetWarehouseCompartments(string productIdentifier, string warehouseIdentifier = null);
    }
}