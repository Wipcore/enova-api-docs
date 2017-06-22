using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Core;

namespace Wipcore.Enova.Api.Abstractions.Internal
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


        /// <summary>
        /// Makes sure the order has a valid warehouse, if it doesn't have one already.
        /// </summary>
        void SetDefaultWarehouse(EnovaOrder order);
    }
}