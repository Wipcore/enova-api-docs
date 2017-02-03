using Wipcore.Core.SessionObjects;

namespace Wipcore.Enova.Api.Abstractions.Interfaces
{
    public interface IWarehouseService
    {
        /// <summary>
        /// Get warehose compartments for the given product. 
        /// </summary>
        /// <param name="productIdentifier"></param>
        /// <param name="warehouseIdentifier">Filter compartments by specific warehose.</param>
        /// <returns></returns>
        BaseObjectList GetWarehouseCompartments(string productIdentifier, string warehouseIdentifier = null);
    }
}