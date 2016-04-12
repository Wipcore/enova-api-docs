using System;
using System.Collections.Generic;
using System.Linq;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Interfaces;
using Wipcore.Enova.Api.Models;
using Wipcore.Enova.Api.Models.Interfaces;
using Wipcore.Enova.Api.WebApi.Helpers;
using Wipcore.Enova.Core;
using Wipcore.Enova.Generics;

namespace Wipcore.eNova.Api.WebApi.EnovaObjectServices
{
    public class CartService : ICartService
    {
        private readonly IContextService _contextService;
        private readonly IMappingToService _mappingToService;


        public CartService(IContextService contextService, IMappingToService mappingToService)
        {
            _contextService = contextService;
            _mappingToService = mappingToService;
        }

        public ICartModel CalculateCart(ICartModel currentCart)
        {
            if (currentCart == null)
                return new CartModel(new List<RowModel>());
            if (currentCart.Rows == null)
                currentCart.Rows = new List<RowModel>();

            var context = _contextService.GetContext();

            var enovaCart = context.FindObject<EnovaCart>(currentCart.Identifier ?? "") ?? EnovaObjectCreationHelper.CreateNew<EnovaCart>(context);
            enovaCart.Edit();

            MapCart(context, enovaCart, currentCart);//make sure all rows match

            enovaCart.Recalculate();
            if (currentCart.Persist)
                enovaCart.Save();
            
            var currency = context.CurrentCurrency;
            decimal rounding, taxAmount;
            int decimals;
            var totalPrice = enovaCart.GetPrice(out taxAmount, out rounding, out decimals, ref currency);

            currentCart.TotalPriceExclTax = totalPrice - taxAmount;
            currentCart.TotalPriceInclTax = totalPrice;

            return currentCart;
        }

        public void MapCart(Context context, EnovaCart enovaCart, ICartModel currentCart)
        {
            enovaCart.Identifier = currentCart.Identifier ?? enovaCart.Identifier;
            var customer = !String.IsNullOrEmpty(currentCart.Customer) ? EnovaCustomer.Find(context, currentCart.Customer) : null;
            if (customer != null)
                enovaCart.Customer = customer;
            else
                currentCart.Customer = enovaCart.Customer?.Identifier;

            enovaCart.Name = currentCart.Name ?? enovaCart.Name;
            currentCart.Name = enovaCart.Name;
            
            currentCart.AdditionalValues = _mappingToService.MapTo(enovaCart, currentCart.AdditionalValues);
            
            //if no rows in given cart, make sure no rows in enova cart
            if (currentCart.Rows == null || !currentCart.Rows.Any())
            {
                enovaCart.DeleteCartItems(null);
            }
            else
            {
                //go through all rows and make sure they match
                foreach (var cartItem in currentCart.Rows)
                {
                    CartItem enovaCartItem = null;
                    switch (cartItem.Type?.ToLower())
                    {
                        case "shipping":
                            enovaCartItem = MapShippingItem(context, enovaCart, cartItem);
                            break;
                        case "payment":
                            enovaCartItem = MapPaymentItem(context, enovaCart, cartItem);
                            break;
                        case "promo":
                            enovaCartItem = MapPromoItem(context, enovaCart, cartItem);
                            break;
                        case "product":
                        default:
                            enovaCartItem = MapProductItem(context, enovaCart, cartItem);
                            break;
                    }

                    cartItem.AdditionalValues = _mappingToService.MapTo(enovaCartItem, cartItem.AdditionalValues);
                }
                
                AddPromoRows(context, enovaCart, currentCart);//promos are added automatically depending on other cartitems

                //any product, payment or shipping items that are not in the model, remove from the cart
                var identifiers = currentCart.Rows.Select(x => x.Identifier).ToList();
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

        public BaseObjectList GetCartsByCustomer(string customerIdentifier)
        {
            var context = _contextService.GetContext();
            var customer = EnovaCustomer.Find(context, customerIdentifier);
            
            var type = typeof(EnovaCart).GetMostDerivedType();
            var carts = context.Search("CustomerID = " + customer.ID, type, null, 0, null, false);
            return carts;
        }

        private PaymentTypeCartItem MapPaymentItem(Context context, EnovaCart enovaCart, IRowModel cartItem)
        {
            var payment = EnovaPaymentType.Find(context, cartItem.Identifier);
            var enovaCartItem = enovaCart.GetCartItems<EnovaPaymentTypeCartItem>().FirstOrDefault(x =>
                String.Equals(x.PaymentType?.Identifier, cartItem.Identifier, StringComparison.InvariantCultureIgnoreCase));

            if (enovaCartItem == null)
            {
                enovaCartItem = EnovaObjectCreationHelper.CreateNew<EnovaPaymentTypeCartItem>(context);
                enovaCartItem.PaymentType = payment;
                enovaCart.AddCartItem(enovaCartItem);
            }

            cartItem.Quantity = 1;
            cartItem.Name = payment.Name;
            cartItem.PriceExclTax = enovaCartItem.GetPrice(includeTax: false);
            cartItem.PriceInclTax = enovaCartItem.GetPrice(includeTax: true);

            return enovaCartItem;
        }

        private ShippingTypeCartItem MapShippingItem(Context context, EnovaCart enovaCart, IRowModel cartItem)
        {
            var shipping = EnovaShippingType.Find(context, cartItem.Identifier);
            var enovaCartItem = enovaCart.GetCartItems<EnovaShippingTypeCartItem>().FirstOrDefault(x => 
                String.Equals(x.ShippingType?.Identifier, cartItem.Identifier, StringComparison.InvariantCultureIgnoreCase));

            if (enovaCartItem == null)
            {
                enovaCartItem = EnovaObjectCreationHelper.CreateNew<EnovaShippingTypeCartItem>(context);
                enovaCartItem.ShippingType = shipping;
                enovaCart.AddCartItem(enovaCartItem);
            }
            
            cartItem.Quantity = 1;
            cartItem.Name = shipping.Name;
            cartItem.PriceExclTax = enovaCartItem.GetPrice(includeTax: false);
            cartItem.PriceInclTax = enovaCartItem.GetPrice(includeTax: true);

            return enovaCartItem;
        }

        private ProductCartItem MapProductItem(Context context, EnovaCart enovaCart, IRowModel cartItem)
        {
            var product = EnovaBaseProduct.Find(context, cartItem.Identifier);
            var enovaCartItem = enovaCart.FindCartItem(product);
            var quantity = cartItem.Quantity > 0 ? cartItem.Quantity : 1;
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

            cartItem.Name = product.Name;
            cartItem.Type = "product";
            cartItem.Quantity = enovaCartItem.Quantity;
            cartItem.PriceExclTax = enovaCartItem.GetPrice(includeTax: false);
            cartItem.PriceInclTax = enovaCartItem.GetPrice(includeTax: true);

            return enovaCartItem;
        }

        private PromoCartItem MapPromoItem(Context context, EnovaCart enovaCart, IRowModel cartItem)
        {
            context.UnlockCampaigns(cartItem.Password);
            enovaCart.Recalculate();

            return enovaCart.GetCartItems<EnovaPromoCartItem>().FirstOrDefault(x => x.Promo?.Password == cartItem.Password);
        }

        private void AddPromoRows(Context context, EnovaCart enovaCart, ICartModel currentCart)
        {
            enovaCart.Recalculate();
            var promoRows = enovaCart.GetCartItems<EnovaPromoCartItem>();
            var rows = currentCart.Rows.ToList();
            foreach (var enovaPromoCartItem in promoRows)
            {
                var newRow = false;
                var cartItem = currentCart.Rows.FirstOrDefault(x => x.Password == enovaPromoCartItem.Promo?.Password);
                if (cartItem == null)
                {
                    cartItem = new RowModel();
                    newRow = true;
                }
                cartItem.Type = "promo";
                cartItem.Identifier = enovaPromoCartItem.Identifier;
                cartItem.Quantity = 1;
                cartItem.Name = enovaPromoCartItem.Name;
                cartItem.PriceExclTax = enovaPromoCartItem.GetPrice(includeTax: false);
                cartItem.PriceInclTax = enovaPromoCartItem.GetPrice(includeTax: true);

                if(newRow)
                    rows.Add(cartItem);
            }

            currentCart.Rows = rows;
        }
    }
}
