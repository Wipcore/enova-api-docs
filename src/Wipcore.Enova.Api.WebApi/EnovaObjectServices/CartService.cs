using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.Extensions.Logging;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.WebApi.Helpers;
using Wipcore.Enova.Api.Interfaces;
using Wipcore.Enova.Api.Models;
using Wipcore.Enova.Api.Models.Cart;
using Wipcore.Enova.Api.Models.Interfaces;
using Wipcore.Enova.Api.Models.Interfaces.Cart;
using Wipcore.Enova.Core;
using Wipcore.Enova.Generics;

namespace Wipcore.Enova.Api.WebApi.EnovaObjectServices
{
    public class CartService : ICartService
    {
        private readonly IContextService _contextService;
        private readonly IMappingToEnovaService _mappingToEnovaService;
        private readonly IAuthService _authService;
        private readonly ILogger _logger;


        public CartService(IContextService contextService, IMappingToEnovaService mappingToEnovaService, IAuthService authService, ILoggerFactory loggerFactory)
        {
            _contextService = contextService;
            _mappingToEnovaService = mappingToEnovaService;
            _authService = authService;
            _logger = loggerFactory.CreateLogger(GetType().Name);
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
            
            cartModel.AdditionalValues = _mappingToEnovaService.MapToEnovaObject(enovaCart, cartModel.AdditionalValues);
            
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

                    cartItem.AdditionalValues = _mappingToEnovaService.MapToEnovaObject(enovaCartItem, cartItem.AdditionalValues);
                }
                
                AddPromoRows(context, enovaCart, cartModel);//promos are added automatically depending on other cartitems

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

        private void AddPromoRows(Context context, EnovaCart enovaCart, ICalculatedCartModel currentCart)
        {
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
