using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Abstractions.Internal;
using Wipcore.Enova.Api.WebApi.Helpers;
using Wipcore.Enova.Core;
using Wipcore.Enova.Generics;

namespace Wipcore.Enova.Api.WebApi.EnovaObjectServices
{
    public class OrderService : IOrderService
    {
        private readonly IContextService _contextService;
        private readonly IMappingToEnovaService _mappingToEnovaService;
        private readonly ICartService _cartService;
        private readonly IConfigurationRoot _configuration;
        private readonly IAuthService _authService;
        private readonly IWarehouseService _warehouseService;
        private readonly ILogger _logger;
        private readonly int _decimalsInAmountString;

        public OrderService( IContextService contextService, IMappingToEnovaService mappingToEnovaService, ICartService cartService, IConfigurationRoot configuration, 
            IAuthService authService, ILoggerFactory loggerFactory, IWarehouseService warehouseService)
        {
            _contextService = contextService;
            _mappingToEnovaService = mappingToEnovaService;
            _cartService = cartService;
            _configuration = configuration;
            _authService = authService;
            _warehouseService = warehouseService;
            _logger = loggerFactory.CreateLogger(GetType().Name);
            _decimalsInAmountString = _configuration.GetValue<int>("EnovaSettings:DecimalsInAmountString", 2);
        }

        /// <summary>
        /// Get orders owned by given customer.
        /// </summary>
        /// <param name="customerIdentifier">Identifier of the customer. Not used if customerId is set.</param>
        /// <param name="customerId"></param>
        /// <param name="shippingStatus">Filter by shippingstatus identifier.</param>
        public BaseObjectList GetOrdersByCustomer(int customerId = 0, string customerIdentifier = null, string shippingStatus = null)
        {
            var context = _contextService.GetContext();
            var customer = customerId > 0 ? EnovaCustomer.Find(context, customerId) : EnovaCustomer.Find(context, customerIdentifier);

            var shippingFilter = String.Empty;
            if (!String.IsNullOrEmpty(shippingStatus))
            {
                var status = EnovaShippingStatus.Find(context, shippingStatus);
                shippingFilter = " AND ShippingStatusID = " + status.ID;
            }

            var type = typeof (EnovaOrder).GetMostDerivedEnovaType();
            var orders = context.Search("CustomerID = " + customer.ID + shippingFilter, type, null, 0, null, false);
            return orders;
        }

        /// <summary>
        /// Get a list of identifiers|names of the valid new shipping statuses for the given order.
        /// </summary>
        public IDictionary<string, string> GetValidShippingStatuses(EnovaOrder order, bool includeCurrentStatus, bool allValidIfNoStatus)
        {
            var context = _contextService.GetContext();
            var currentStatus = order.ShippingStatus?.Identifier;
            if (currentStatus == null)
                return allValidIfNoStatus ? context.GetAllObjects<EnovaShippingStatus>().Where(x => !String.IsNullOrEmpty(x?.Identifier)).ToDictionary(k => k.Identifier, v => v.Name) : new Dictionary<string, string>();
            
            var destinations = context.Search<EnovaConfigShippingStatusRule>("Allow = 1 AND Enabled = 1 AND SourceStatus = "+currentStatus).Select(x => x.DestinationStatus).ToList();

            if(includeCurrentStatus)
                destinations.Add(order.ShippingStatus.Identifier);

            var statuses = context.FindObjects<EnovaShippingStatus>(destinations).Where(x => !String.IsNullOrEmpty(x?.Identifier)).ToDictionary(k => k.Identifier, v => v.Name);
            return statuses;
        }
        
    }
}
