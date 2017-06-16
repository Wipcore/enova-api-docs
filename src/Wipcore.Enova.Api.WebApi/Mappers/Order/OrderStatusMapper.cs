﻿using System;
using System.Collections.Generic;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Abstractions.Interfaces;
using Wipcore.Enova.Core;
using Wipcore.Enova.Generics;

namespace Wipcore.eNova.Api.WebApi.Mappers.Order
{
    public class OrderStatusMapper : IPropertyMapper
    {
        private readonly IWarehouseService _warehouseService;
        public OrderStatusMapper(IWarehouseService warehouseService)
        {
            _warehouseService = warehouseService;
        }

        public List<string> Names => new List<string>() { "ShippingStatus" };
        public Type CmoType => typeof(CmoEnovaOrder);
        public Type Type => typeof(EnovaOrder);
        public bool InheritMapper => true;
        public bool FlattenMapping => false;
        public int Priority => 0;
        public MapType MapType => MapType.MapFromAndToEnovaAllowed;

        public bool PostSaveSet => true;
        
        public object GetEnovaProperty(BaseObject obj, string propertyName, List<EnovaLanguage> mappingLanguages)
        {
            var order = (EnovaOrder)obj;
            return order.ShippingStatus?.Identifier;
        }

        public void SetEnovaProperty(BaseObject obj, string propertyName, object value, IDictionary<string, object> otherValues)
        {
            var newStatus = value?.ToString().Trim();
            if (String.IsNullOrEmpty(newStatus))
                return;

            var order = (EnovaOrder)obj;

            if (order.ShippingStatus?.Identifier == newStatus)
                return;

            _warehouseService.SetDefaultWarehouse(order);//must have a warehouse, or it will fail

            var shippingStatus = order.GetContext().FindObject<EnovaShippingStatus>(newStatus);
            if (order.ShippingStatus == null)
            {
                order.Edit();
                order.ShippingStatus = shippingStatus;
                order.Save();
            }
            else
                order.ChangeShippingStatus(shippingStatus, null);//TODO maybe insert paymenthandlers by configuration here
        }
    }
}
