using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Wipcore.Core.SessionObjects;
using Wipcore.eNova.Api.WebApi.Helpers;
using Wipcore.Enova.Api.Interfaces;
using Wipcore.Enova.Api.Models;
using Wipcore.Enova.Api.Models.Cart;
using Wipcore.Enova.Api.Models.Interfaces;
using Wipcore.Enova.Api.Models.Interfaces.Cart;
using Wipcore.Enova.Api.WebApi.Helpers;
using Wipcore.Enova.Core;
using Wipcore.Enova.Generics;

namespace Wipcore.eNova.Api.WebApi.EnovaObjectServices
{
    public class OrderService : IOrderService
    {
        private readonly IContextService _contextService;
        private readonly IMappingToEnovaService _mappingToEnovaService;
        private readonly ICartService _cartService;
        private readonly IConfigurationRoot _configuration;
        private readonly IAuthService _authService;
        private readonly ILogger _logger;

        public OrderService( IContextService contextService, IMappingToEnovaService mappingToEnovaService, ICartService cartService, IConfigurationRoot configuration, 
            IAuthService authService, ILoggerFactory loggerFactory)
        {
            _contextService = contextService;
            _mappingToEnovaService = mappingToEnovaService;
            _cartService = cartService;
            _configuration = configuration;
            _authService = authService;
            _logger = loggerFactory.CreateLogger(GetType().Name);
        }

        /// <summary>
        /// Get orders owned by given customer.
        /// </summary>
        /// <param name="customerIdentifier"></param>
        /// <param name="shippingStatus">Filter by shippingstatus identifier.</param>
        /// <returns></returns>
        public BaseObjectList GetOrdersByCustomer(string customerIdentifier, string shippingStatus = null)
        {
            var context = _contextService.GetContext();
            var customer = EnovaCustomer.Find(context, customerIdentifier);

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
        /// Get a list of identifiers of the valid new shipping statuses for the given order.
        /// </summary>
        public IEnumerable<string> GetValidShippingStatuses(EnovaOrder order)
        {
            var currentStatus = order.ShippingStatus?.Identifier;
            if(currentStatus == null)
                return new List<string>();

            var context = _contextService.GetContext();
            var destinations = context.Search<EnovaConfigShippingStatusRule>("Allow = 1 AND Enabled = 1 AND SourceStatus = "+currentStatus).Select(x => x.DestinationStatus);
            return destinations;
        }

        /// <summary>
        /// Create a new order or update an order. Updates cannot change price/quantity of orderrows, that requires a new order.
        /// </summary>
        public ICartModel SaveOrder(ICartModel cartModel)
        {
            if (cartModel == null)
                return new CartModel(new List<CalculatedCartRowModel>());
            if (cartModel.Rows == null)
                cartModel.Rows = new List<CalculatedCartRowModel>();

            if (cartModel.Identifier == String.Empty)
                cartModel.Identifier = null;

            var context = _contextService.GetContext();
            
            var enovaOrder = context.FindObject<EnovaOrder>(cartModel.Identifier);

            if (!_authService.AuthorizeUpdate(enovaOrder?.Customer?.Identifier, cartModel.Customer))
                throw new HttpException(HttpStatusCode.Unauthorized, "A customer can only update it's own order.");

            var calculatedOrder = CalculatedCartModel.CreateFrom(cartModel);

            if (enovaOrder == null)//if new then create order from cart
            {
                var identifier = calculatedOrder.Identifier;
                calculatedOrder.Identifier = null;
                var additionalValues = cartModel.AdditionalValues;
                calculatedOrder.AdditionalValues = null;

                if (String.IsNullOrEmpty(identifier))
                    identifier = EnovaCommonFunctions.GetSequenceNumber(context, SystemRunningMode.Remote, SequenceType.OrderIdentifier);

                var dummyCart = EnovaObjectCreationHelper.CreateNew<EnovaCart>(context);
                _cartService.MapCart(context, dummyCart, calculatedOrder);

                enovaOrder = EnovaObjectCreationHelper.CreateNew<EnovaOrder>(context, dummyCart);
                enovaOrder.Identifier = calculatedOrder.Identifier = identifier;
                calculatedOrder.AdditionalValues = additionalValues;

                var warehouseSetting = context.FindObject<EnovaLocalSystemSettings>("LOCAL_PRIMARY_WAREHOUSE");
                var defaultWarehouse = warehouseSetting?.Value?.Split(';')?.FirstOrDefault() ?? "DEFAULT_WAREHOUSE";
                enovaOrder.Warehouse = EnovaWarehouse.Find(context, defaultWarehouse);

                enovaOrder.Recalculate();
            }

            SetStatus(context, enovaOrder, calculatedOrder);

            Map(context, enovaOrder, calculatedOrder);
            
            //calculate prices
            var currency = context.CurrentCurrency;
            decimal taxAmount;
            int decimals;
            double roundingFactor;
            var totalPrice = enovaOrder.GetSum(out taxAmount, out decimals, ref currency, out roundingFactor);

            calculatedOrder.TotalPriceExclTax = totalPrice;
            calculatedOrder.TotalPriceInclTax = totalPrice + taxAmount;

            if (cartModel.Persist)
            {
                var newOrder = enovaOrder.ID == default(int);
                enovaOrder.Save();
                _logger.LogInformation("{0} {1} order with Identifier {2}, Type: {3} and Values: {4}",_authService.LogUser(), 
                    newOrder ? "Created" : "Updated", enovaOrder.Identifier, enovaOrder.GetType().Name, cartModel.ToString());
            }

            return calculatedOrder;
        }

        private void SetStatus(Context context, EnovaOrder enovaOrder, ICalculatedCartModel model)
        {
            if (enovaOrder.ShippingStatus == null)//if none, set to given status or default new
            {
                if(!enovaOrder.IsBeingEdited)
                    enovaOrder.Edit();
                var newShippingIdentifier = model.Status ?? _configuration["EnovaSettings:NewShippingStatus"] ?? "NEW_INTERNET";
                var shippingStatus = EnovaShippingStatus.Find(context, newShippingIdentifier);
                enovaOrder.ShippingStatus = shippingStatus;
            }
            else if (!String.IsNullOrEmpty(model.Status))//if status specified that's different from current, then change it
            {
                var currentStatus = enovaOrder.ShippingStatus?.Identifier;
                if (!String.Equals(currentStatus, model.Status, StringComparison.InvariantCultureIgnoreCase))
                {
                    enovaOrder.ChangeShippingStatus(EnovaShippingStatus.Find(context, model.Status), null);
                }
            }

            model.Status = enovaOrder.ShippingStatus?.Identifier;
        }

        /// <summary>
        /// Mapping order. Maps extra properties (additional values) on order and orderrows. Does not otherwise change orderrows.
        /// </summary>
        private void Map(Context context, EnovaOrder enovaOrder, ICalculatedCartModel model)
        {
            var orderRows = enovaOrder.OrderItems;
            if (!enovaOrder.IsBeingEdited)
                enovaOrder.Edit();

            model.AdditionalValues = _mappingToEnovaService.MapToEnovaObject(enovaOrder, model.AdditionalValues);
            model.Customer = enovaOrder.Customer?.Identifier;
            var mappedRows = new List<CalculatedCartRowModel>();
            
            foreach (var orderItem in orderRows.OfType<EnovaProductOrderItem>())
            {
                var row = (CalculatedCartRowModel)model.Rows.FirstOrDefault(x => x.Type == RowType.Product && 
                    String.Equals(orderItem.ProductIdentifier, x.Identifier, StringComparison.OrdinalIgnoreCase)) ?? new CalculatedCartRowModel();
                row.Identifier = orderItem.ProductIdentifier;
                row.Quantity = orderItem.OrderedQuantity;
                row.PriceExclTax = orderItem.GetPrice(false);
                row.PriceInclTax = orderItem.GetPrice(true);
                row.Type = RowType.Product;
                row.Name = orderItem.Name;

                row.AdditionalValues = _mappingToEnovaService.MapToEnovaObject(orderItem, row.AdditionalValues);
                mappedRows.Add(row);
            }

            foreach (var orderItem in orderRows.OfType<EnovaShippingTypeOrderItem>())
            {
                var row = (CalculatedCartRowModel)model.Rows.FirstOrDefault(x => x.Type == RowType.Shipping &&
                    String.Equals(orderItem.ShippingType?.Identifier, x.Identifier, StringComparison.OrdinalIgnoreCase)) ?? new CalculatedCartRowModel();
                row.Identifier = orderItem.ShippingType?.Identifier;
                row.Quantity = orderItem.OrderedQuantity;
                row.PriceExclTax = orderItem.GetPrice(false);
                row.PriceInclTax = orderItem.GetPrice(true);
                row.Type = RowType.Shipping;
                row.Name = orderItem.Name;

                row.AdditionalValues = _mappingToEnovaService.MapToEnovaObject(orderItem, row.AdditionalValues);
                mappedRows.Add(row);
            }

            foreach (var orderItem in orderRows.OfType<EnovaPaymentTypeOrderItem>())
            {
                var row = (CalculatedCartRowModel)model.Rows.FirstOrDefault(x => x.Type == RowType.Payment &&
                    String.Equals(orderItem.PaymentType?.Identifier, x.Identifier, StringComparison.OrdinalIgnoreCase)) ?? new CalculatedCartRowModel();
                row.Identifier = orderItem.PaymentType?.Identifier;
                row.Quantity = orderItem.OrderedQuantity;
                row.PriceExclTax = orderItem.GetPrice(false);
                row.PriceInclTax = orderItem.GetPrice(true);
                row.Type = RowType.Payment;
                row.Name = orderItem.Name;

                row.AdditionalValues = _mappingToEnovaService.MapToEnovaObject(orderItem, row.AdditionalValues);
                mappedRows.Add(row);
            }

            foreach (var orderItem in orderRows.OfType<EnovaPromoOrderItem>())
            {
                var row = (CalculatedCartRowModel)model.Rows.FirstOrDefault(x => x.Type == RowType.Promo && (x.Password == orderItem.Promo?.Password ||
                    String.Equals(orderItem.Promo?.Identifier, x.Identifier, StringComparison.OrdinalIgnoreCase))) ?? new CalculatedCartRowModel();
                row.Identifier = orderItem.Promo?.Identifier;
                row.Quantity = orderItem.OrderedQuantity;
                row.PriceExclTax = orderItem.GetPrice(false);
                row.PriceInclTax = orderItem.GetPrice(true);
                row.Type = RowType.Promo;
                row.Name = orderItem.Name;

                row.AdditionalValues = _mappingToEnovaService.MapToEnovaObject(orderItem, row.AdditionalValues);
                mappedRows.Add(row);
            }

            model.Rows = mappedRows;
        }
    }
}
