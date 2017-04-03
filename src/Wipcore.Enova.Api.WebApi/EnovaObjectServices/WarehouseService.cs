using System;
using System.Linq;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Abstractions.Interfaces;
using Wipcore.Enova.Core;
using Wipcore.Enova.Generics;

namespace Wipcore.Enova.Api.WebApi.EnovaObjectServices
{
    public class WarehouseService : IWarehouseService
    {
        private readonly IContextService _contextService;

        public WarehouseService(IContextService contextService)
        {
            _contextService = contextService;
        }

        /// <summary>
        /// Get warehose compartments for the given product. 
        /// </summary>
        /// <param name="productIdentifier"></param>
        /// <param name="warehouseIdentifier">Filter compartments by specific warehose.</param>
        /// <returns></returns>
        public BaseObjectList GetWarehouseCompartments(string productIdentifier, string warehouseIdentifier = null)
        {
            var context = _contextService.GetContext();
            var product = EnovaBaseProduct.Find(context, productIdentifier);

            EnovaWarehouse warehouse = null;
            if(!String.IsNullOrEmpty(warehouseIdentifier))
                warehouse = EnovaWarehouse.Find(context, warehouseIdentifier);

            return product.GetWarehouseCompartments(typeof (EnovaWarehouseCompartment), warehouse);
        }

        /// <summary>
        /// Makes sure the order has a valid warehouse, if it doesn't have one already.
        /// </summary>
        public void SetDefaultWarehouse(EnovaOrder order)
        {
            if(order.Warehouse != null)
                return;

            var context = order.GetContext();
            var warehouseSetting = context.FindObject<EnovaLocalSystemSettings>("LOCAL_PRIMARY_WAREHOUSE");
            var defaultWarehouse = warehouseSetting?.Value?.Split(';')?.FirstOrDefault() ?? "DEFAULT_WAREHOUSE";

            var isBeingEdited = order.IsBeingEdited;
            if(!isBeingEdited)
                order.Edit();

            order.Warehouse = EnovaWarehouse.Find(context, defaultWarehouse);

            if (!isBeingEdited)
                order.Save();
        }
    }
}
