using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Abstractions;
using Wipcore.Enova.Api.Abstractions.Internal;
using Wipcore.Enova.Api.Abstractions.Models;
using Wipcore.Enova.Api.WebApi.Helpers;
using Wipcore.Enova.Core;
using Wipcore.Enova.Generics;

namespace Wipcore.Enova.Api.WebApi.EnovaObjectServices
{
    public class CartService : ICartService
    {
        private readonly IContextService _contextService;
        private readonly IMappingToEnovaService _mappingToEnovaService;
        private readonly IMappingFromEnovaService _mappingFromEnovaService;
        private readonly IAuthService _authService;
        private readonly IConfigurationRoot _configuration;
        private readonly ILogger _logger;
        private readonly int _decimalsInAmountString;


        public CartService(IContextService contextService, IMappingToEnovaService mappingToEnovaService, IMappingFromEnovaService mappingFromEnovaService, IAuthService authService, 
            ILoggerFactory loggerFactory, IConfigurationRoot configuration)
        {
            _contextService = contextService;
            _mappingToEnovaService = mappingToEnovaService;
            _mappingFromEnovaService = mappingFromEnovaService;
            _authService = authService;
            _configuration = configuration;
            _logger = loggerFactory.CreateLogger(GetType().Name);
            _decimalsInAmountString = _configuration.GetValue<int>("EnovaSettings:DecimalsInAmountString", 2);
        }


        /// <summary>
        /// Creates an order from a mapping to cart by given values.
        /// </summary>
        public int CreateOrderFromCart(ContextModel requestContext, Dictionary<string, object> values)
        {
            var context = _contextService.GetContext();
            var id = values.GetValueInsensitive<int>("id");
            var identifier = values.GetValueInsensitive<string>("identifier");

            if (identifier == String.Empty)
                identifier = null;

            var cart = context.Find<EnovaCart>(id, identifier) ?? EnovaObjectMakerHelper.CreateNew<EnovaCart>(context);
            cart.Edit();

            _mappingToEnovaService.MapToEnovaObject(cart, values);

            var order = EnovaObjectMakerHelper.CreateNew<EnovaOrder>(context, cart);
            order.Identifier = EnovaCommonFunctions.GetSequenceNumber(context, SystemRunningMode.Remote, SequenceType.OrderIdentifier);

            var newShippingIdentifier = _configuration["EnovaSettings:NewShippingStatus"] ?? "NEW_INTERNET";
            var shippingStatus = EnovaShippingStatus.Find(context, newShippingIdentifier);

            order.ShippingStatus = shippingStatus;
            order.Save();

            return order.ID;
        }

        /// <summary>
        /// Get all carts belonging to given customer.
        /// </summary>
        public BaseObjectList GetCartsByCustomer(string customerIdentifier = null, int customerId = 0)
        {
            var context = _contextService.GetContext();
            var customer = customerId > 0 ? EnovaCustomer.Find(context, customerId) : EnovaCustomer.Find(context, customerIdentifier);

            var type = typeof(EnovaCart).GetMostDerivedEnovaType();
            var carts = context.Search("CustomerID = " + customer.ID, type, null, 0, null, false);
            return carts;
        }
    }
}
