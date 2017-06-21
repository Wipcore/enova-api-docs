using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Abstractions.Interfaces;
using Wipcore.Enova.Api.Abstractions.Interfaces.Cart;
using Wipcore.Enova.Api.Abstractions.Internal;
using Wipcore.Enova.Api.Abstractions.Models;
using Wipcore.Enova.Api.WebApi.Helpers;
using Wipcore.Enova.Core;
using Wipcore.Enova.Generics;
using Wipcore.Library;

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

            var cart = context.FindObject<EnovaCart>(id) ?? context.FindObject<EnovaCart>(identifier) ?? EnovaObjectCreationHelper.CreateNew<EnovaCart>(context);
            cart.Edit();

            _mappingToEnovaService.MapToEnovaObject(cart, values);

            var order = EnovaObjectCreationHelper.CreateNew<EnovaOrder>(context, cart);
            order.Identifier = EnovaCommonFunctions.GetSequenceNumber(context, SystemRunningMode.Remote, SequenceType.OrderIdentifier);

            var newShippingIdentifier = _configuration["EnovaSettings:NewShippingStatus"] ?? "NEW_INTERNET";
            var shippingStatus = EnovaShippingStatus.Find(context, newShippingIdentifier);

            order.ShippingStatus = shippingStatus;
            order.Save();

            return order.ID;
        }

        /// <summary>
        /// Get an existing cart, mapped to a model.
        /// </summary>
        public ICalculatedCartModel GetCart(string identifier = null, int id = 0)
        {
            var context = _contextService.GetContext();
            var enovaCart = context.FindObject<EnovaCart>(identifier) ?? context.FindObject<EnovaCart>(id);
            if (enovaCart == null)
                return null;

            var currency = context.CurrentCurrency;
            decimal rounding, taxAmount;
            int decimals;
            var totalPrice = enovaCart.GetPrice(out taxAmount, out rounding, out decimals, ref currency);
            
            var rows = new List<CalculatedCartRowModel>();
            var model = new CalculatedCartModel(rows) {Customer = enovaCart.Customer?.Identifier, Identifier = enovaCart.Identifier, TotalPriceExclTax = totalPrice - taxAmount, TotalPriceInclTax = totalPrice };
            model.TotalPriceExclTaxString = context.AmountToString(model.TotalPriceExclTax, currency, _decimalsInAmountString, true, true);
            model.TotalPriceInclTaxString = context.AmountToString(model.TotalPriceInclTax, currency, _decimalsInAmountString, true, true);

            var shipping = enovaCart.GetCartItems<EnovaShippingTypeCartItem>().FirstOrDefault();
            if (shipping?.ShippingType?.Identifier != null)
            {
                var shippingModel = new CalculatedCartRowModel() {Identifier = shipping.ShippingType.Identifier, Type = RowType.Shipping};
                MapShippingItem(context, enovaCart, shippingModel);
                rows.Add(shippingModel);
            }

            var payment = enovaCart.GetCartItems<EnovaPaymentTypeCartItem>().FirstOrDefault();
            if (payment?.PaymentType?.Identifier != null)
            {
                var paymentModel = new CalculatedCartRowModel() { Identifier = payment.PaymentType.Identifier, Type = RowType.Payment};
                MapPaymentItem(context, enovaCart, paymentModel);
                rows.Add(paymentModel);
            }

            foreach (var cartItem in enovaCart.GetCartItems<EnovaProductCartItem>())
            {
                var productModel = new CalculatedCartRowModel() { Identifier = cartItem.ProductIdentifier, Type = RowType.Product, Quantity = cartItem.Quantity};
                MapProductItem(context, enovaCart, productModel);
                rows.Add(productModel);
            }
            
            AddPromoRows(enovaCart, model);
            
            return model;
        }


        /// <summary>
        /// Maps given model to a cart in Enova and returns a model with prices specified.  
        /// </summary>
        public ICalculatedCartModel CalculateCart(ICartModel currentCart)
        {
            if (currentCart == null)
                return new CalculatedCartModel(new List<CalculatedCartRowModel>());
            if (currentCart.Rows == null)
                currentCart.Rows = new List<CalculatedCartRowModel>();
            if (currentCart.Identifier == String.Empty)
                currentCart.Identifier = null;

            var context = _contextService.GetContext();

            var enovaCart = context.FindObject<EnovaCart>(currentCart.Identifier) ?? EnovaObjectCreationHelper.CreateNew<EnovaCart>(context);

            if (!_authService.AuthorizeUpdate(enovaCart.Customer?.Identifier, currentCart.Customer))
                throw new HttpException(HttpStatusCode.Unauthorized, "A customer can only update it's own cart.");

            enovaCart.Edit();

            var calculatedCart = CalculatedCartModel.CreateFrom(currentCart);
            MapCart(context, enovaCart, calculatedCart);//make sure all rows match

            enovaCart.Recalculate();
            
            var currency = context.CurrentCurrency;
            decimal rounding, taxAmount;
            int decimals;
            var totalPrice = enovaCart.GetPrice(out taxAmount, out rounding, out decimals, ref currency);

            calculatedCart.TotalPriceExclTax = totalPrice - taxAmount;
            calculatedCart.TotalPriceInclTax = totalPrice;
            calculatedCart.TotalPriceExclTaxString = context.AmountToString(calculatedCart.TotalPriceExclTax, currency, _decimalsInAmountString, true, true);
            calculatedCart.TotalPriceInclTaxString = context.AmountToString(calculatedCart.TotalPriceInclTax, currency, _decimalsInAmountString, true, true);

            calculatedCart.Rows.Cast<ICalculatedCartRowModel>().ForEach(x =>
            {
                x.PriceExclTaxString = context.AmountToString(x.PriceExclTax, currency, _decimalsInAmountString, true, true);
                x.PriceInclTaxString = context.AmountToString(x.PriceInclTax, currency, _decimalsInAmountString, true, true);
            });

            if (currentCart.Persist)
            {
                var newCart = enovaCart.ID == default(int);
                enovaCart.Save();
                _logger.LogInformation("{0} {1} cart with Identifier {2}, Type: {3} and Values: {4}", 
                    _authService.LogUser(), newCart ? "Created" : "Updated", enovaCart.Identifier, enovaCart.GetType().Name, currentCart.ToString());
            }

            return calculatedCart;
        }

        /// <summary>
        /// Map order rows between cart and model.
        /// </summary>
        public void MapCart(Context context, EnovaCart enovaCart, ICalculatedCartModel cartModel)
        {
            enovaCart.Identifier = cartModel.Identifier ?? enovaCart.Identifier;
            
            //set customer
            var customerIdentifier = !String.IsNullOrEmpty(cartModel.Customer) ? cartModel.Customer : _authService.GetLoggedInIdentifier();
            var customer = context.FindObject<EnovaCustomer>(customerIdentifier);
            if (customer != null)
                enovaCart.Customer = customer;
            cartModel.Customer = enovaCart.Customer?.Identifier;
            
            _mappingToEnovaService.MapToEnovaObject(enovaCart, cartModel.AdditionalValues);
            
            //if no rows in given cart, make sure no rows in enova cart
            if (cartModel.Rows == null || !cartModel.Rows.Any())
            {
                enovaCart.DeleteCartItems(null);
            }
            else
            {
                //go through all rows and make sure they match
                foreach (var cartItem in cartModel.Rows.Cast<ICalculatedCartRowModel>())
                {
                    CartItem enovaCartItem = null;
                    switch (cartItem.Type)
                    {
                        case RowType.Shipping:
                            enovaCartItem = MapShippingItem(context, enovaCart, cartItem);
                            break;
                        case RowType.Payment:
                            enovaCartItem = MapPaymentItem(context, enovaCart, cartItem);
                            break;
                        case RowType.Promo:
                            enovaCartItem = MapPromoItem(context, enovaCart, cartItem);
                            break;
                        case RowType.Product:
                        default:
                            enovaCartItem = MapProductItem(context, enovaCart, cartItem);
                            break;
                    }

                    _mappingToEnovaService.MapToEnovaObject(enovaCartItem, cartItem.AdditionalValues);
                }
                
                AddPromoRows(enovaCart, cartModel);//promos are added automatically depending on other cartitems

                //any product, payment or shipping items that are not in the model, remove from the cart
                var identifiers = cartModel.Rows.Select(x => x.Identifier).ToList();
                var itemsToRemove = new List<CartItem>();
                itemsToRemove.AddRange(enovaCart.GetCartItems<EnovaProductCartItem>().Where(x => 
                    !identifiers.Contains(x.Product?.Identifier ?? String.Empty, StringComparer.InvariantCultureIgnoreCase)));
                itemsToRemove.AddRange(enovaCart.GetCartItems<EnovaShippingTypeCartItem>().Where(x => 
                    !identifiers.Contains(x.ShippingType?.Identifier ?? String.Empty, StringComparer.InvariantCultureIgnoreCase)));
                itemsToRemove.AddRange(enovaCart.GetCartItems<EnovaPaymentTypeCartItem>().Where(x => 
                    !identifiers.Contains(x.PaymentType?.Identifier ?? String.Empty, StringComparer.InvariantCultureIgnoreCase)));

                itemsToRemove.ForEach(enovaCart.DeleteCartItem);
            }
        }

        /// <summary>
        /// Get all carts belonging to given customer.
        /// </summary>
        public BaseObjectList GetCartsByCustomer(string customerIdentifier)
        {
            var context = _contextService.GetContext();
            var customer = EnovaCustomer.Find(context, customerIdentifier);
            
            var type = typeof(EnovaCart).GetMostDerivedEnovaType();
            var carts = context.Search("CustomerID = " + customer.ID, type, null, 0, null, false);
            return carts;
        }

        private PaymentTypeCartItem MapPaymentItem(Context context, EnovaCart enovaCart, ICalculatedCartRowModel calculatedCartItem)
        {
            var payment = EnovaPaymentType.Find(context, calculatedCartItem.Identifier);
            var enovaCartItem = enovaCart.GetCartItems<EnovaPaymentTypeCartItem>().FirstOrDefault(x =>
                String.Equals(x.PaymentType?.Identifier, calculatedCartItem.Identifier, StringComparison.InvariantCultureIgnoreCase));

            if (enovaCartItem == null)
            {
                enovaCartItem = EnovaObjectCreationHelper.CreateNew<EnovaPaymentTypeCartItem>(context);
                enovaCartItem.PaymentType = payment;
                enovaCart.AddCartItem(enovaCartItem);
            }

            calculatedCartItem.Quantity = 1;
            calculatedCartItem.Name = payment.Name;
            calculatedCartItem.PriceExclTax = enovaCartItem.GetPrice(includeTax: false);
            calculatedCartItem.PriceInclTax = enovaCartItem.GetPrice(includeTax: true);
            calculatedCartItem.Identifier = enovaCartItem.PaymentType?.Identifier;

            return enovaCartItem;
        }

        private ShippingTypeCartItem MapShippingItem(Context context, EnovaCart enovaCart, ICalculatedCartRowModel calculatedCartItem)
        {
            var shipping = EnovaShippingType.Find(context, calculatedCartItem.Identifier);
            var enovaCartItem = enovaCart.GetCartItems<EnovaShippingTypeCartItem>().FirstOrDefault(x => 
                String.Equals(x.ShippingType?.Identifier, calculatedCartItem.Identifier, StringComparison.InvariantCultureIgnoreCase));

            if (enovaCartItem == null)
            {
                enovaCartItem = EnovaObjectCreationHelper.CreateNew<EnovaShippingTypeCartItem>(context);
                enovaCartItem.ShippingType = shipping;
                enovaCart.AddCartItem(enovaCartItem);
            }
            
            calculatedCartItem.Quantity = 1;
            calculatedCartItem.Name = shipping.Name;
            calculatedCartItem.PriceExclTax = enovaCartItem.GetPrice(includeTax: false);
            calculatedCartItem.PriceInclTax = enovaCartItem.GetPrice(includeTax: true);
            calculatedCartItem.Identifier = enovaCartItem.ShippingType?.Identifier;

            return enovaCartItem;
        }

        private ProductCartItem MapProductItem(Context context, EnovaCart enovaCart, ICalculatedCartRowModel calculatedCartItem)
        {
            var product = EnovaBaseProduct.Find(context, calculatedCartItem.Identifier);
            var enovaCartItem = enovaCart.FindCartItem(product);
            var quantity = calculatedCartItem.Quantity > 0 ? calculatedCartItem.Quantity : 1;
            if (enovaCartItem != null && enovaCartItem.Quantity != quantity)
            {
                enovaCartItem.Quantity = quantity;
            }
            else if (enovaCartItem == null)
            {
                enovaCartItem = EnovaObjectCreationHelper.CreateNew<EnovaProductCartItem>(context);
                enovaCartItem.Product = product;
                enovaCartItem.Quantity = quantity;
                enovaCart.AddCartItem(enovaCartItem);
            }

            calculatedCartItem.Name = product.Name;
            calculatedCartItem.Type = RowType.Product;
            calculatedCartItem.Quantity = enovaCartItem.Quantity;
            calculatedCartItem.PriceExclTax = enovaCartItem.GetPrice(includeTax: false);
            calculatedCartItem.PriceInclTax = enovaCartItem.GetPrice(includeTax: true);
            calculatedCartItem.Identifier = enovaCartItem.ProductIdentifier;

            return enovaCartItem;
        }

        private PromoCartItem MapPromoItem(Context context, EnovaCart enovaCart, ICalculatedCartRowModel calculatedCartItem)
        {
            context.UnlockCampaigns(calculatedCartItem.Password);
            enovaCart.Recalculate();

            return enovaCart.GetCartItems<EnovaPromoCartItem>().FirstOrDefault(x => x.Promo?.Password == calculatedCartItem.Password);
        }

        private void AddPromoRows(EnovaCart enovaCart, ICalculatedCartModel currentCart)
        {
            if(enovaCart.IsBeingEdited)
                enovaCart.Recalculate();

            var promoRows = enovaCart.GetCartItems<EnovaPromoCartItem>();
            var rows = currentCart.Rows.ToList();
            foreach (var enovaPromoCartItem in promoRows)
            {
                var newRow = false;
                var cartItem = currentCart.Rows.FirstOrDefault(x => x.Password == enovaPromoCartItem.Promo?.Password) as ICalculatedCartRowModel;
                if (cartItem == null)
                {
                    cartItem = new CalculatedCartRowModel();
                    newRow = true;
                }
                cartItem.Type = RowType.Promo;
                cartItem.Identifier = enovaPromoCartItem.Promo?.Identifier;
                cartItem.Quantity = 1;
                cartItem.Name = enovaPromoCartItem.Name;
                cartItem.PriceExclTax = enovaPromoCartItem.GetPrice(includeTax: false);
                cartItem.PriceInclTax = enovaPromoCartItem.GetPrice(includeTax: true);

                if(newRow)
                    rows.Add(cartItem);
            }

            var promoProductRows = enovaCart.GetCartItems<EnovaPromoProductCartItem>();
            foreach (var enovaPromoCartItem in promoProductRows)
            {
                var newRow = false;
                var cartItem = currentCart.Rows.FirstOrDefault(x => x.Password == enovaPromoCartItem.Promo?.Password) as ICalculatedCartRowModel;
                if (cartItem == null)
                {
                    cartItem = new CalculatedCartRowModel();
                    newRow = true;
                }
                cartItem.Type = RowType.Promo;
                cartItem.Identifier = enovaPromoCartItem.Promo?.Identifier;
                cartItem.Quantity = 1;
                cartItem.Name = enovaPromoCartItem.Name;
                cartItem.PriceExclTax = enovaPromoCartItem.GetPrice(includeTax: false);
                cartItem.PriceInclTax = enovaPromoCartItem.GetPrice(includeTax: true);

                if (newRow)
                    rows.Add(cartItem);
            }

            currentCart.Rows = rows;
        }
    }
}
