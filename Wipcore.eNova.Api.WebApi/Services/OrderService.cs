using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.WebApi.Helpers;
using Wipcore.Enova.Api.WebApi.Mappers;
using Wipcore.Enova.Api.Models;
using Wipcore.Enova.Api.Interfaces;
using Wipcore.Enova.Core;
using Wipcore.Enova.Generics;

namespace Wipcore.Enova.Api.WebApi.Services
{
    public class OrderService : IOrderService
    {
        private readonly IContextService _contextService;
        private readonly IMappingToService _mappingToService;
        private readonly ICartService _cartService;

        public OrderService( IContextService contextService, IMappingToService mappingToService, ICartService cartService)
        {
            _contextService = contextService;
            _mappingToService = mappingToService;
            _cartService = cartService;
        }

        public ICartModel CreateOrder(ICartModel cartModel)
        {
            if (cartModel == null)
                return new CartModel(new List<RowModel>());
            if (cartModel.Rows == null)
                cartModel.Rows = new List<RowModel>();

            var context = _contextService.GetContext();
            
            //if new then make a cart from it first, if old then edit old order
            var enovaOrder = context.FindObject<EnovaOrder>(cartModel.Identifier ?? "");
            if (enovaOrder == null)
            {
                var identifier = cartModel.Identifier;
                cartModel.Identifier = null;
                var dummyCart = EnovaObjectCreationHelper.CreateNew<EnovaCart>(context);
                _cartService.MapCart(context, dummyCart, cartModel);
                enovaOrder = EnovaObjectCreationHelper.CreateNew<EnovaOrder>(context, dummyCart);
                enovaOrder.Identifier = cartModel.Identifier = identifier;
            }
            else
                enovaOrder.Edit();

            Map(context, enovaOrder, cartModel);
            
            enovaOrder.Recalculate();
            if (cartModel.Persist)
                enovaOrder.Save();

            var enovaOrderItems = enovaOrder.GetOrderItems<EnovaProductOrderItem>().ToList();
            foreach (var row in cartModel.Rows) //fill in price information
            {
                var item = enovaOrderItems.First(x => x.Product.Identifier == row.Product);
                row.PriceExclTax = item.GetPrice(false);
                row.PriceInclTax = item.GetPrice(true);
            }

            var currency = context.CurrentCurrency;
            decimal taxAmount;
            int decimals;
            double roundingFactor;
            var totalPrice = enovaOrder.GetSum(out taxAmount, out decimals, ref currency, out roundingFactor);

            cartModel.TotalPriceExclTax = totalPrice;
            cartModel.TotalPriceInclTax = totalPrice + taxAmount;

            return cartModel;
        }

        private void Map(Context context, EnovaOrder enovaOrder, ICartModel currentCart)
        {
            enovaOrder.Identifier = currentCart.Identifier ?? enovaOrder.Identifier;
            var customer = !String.IsNullOrEmpty(currentCart.Customer) ? EnovaCustomer.Find(context, currentCart.Customer) : null;
            if (customer != null)
                enovaOrder.Customer = customer;
            else
                currentCart.Customer = enovaOrder.Customer?.Identifier;
            
            currentCart.AdditionalValues = _mappingToService.MapTo(enovaOrder, currentCart.AdditionalValues);
            
            //if no rows in given cart, make sure no rows in enova cart
            if (currentCart.Rows == null || !currentCart.Rows.Any())
            {
                enovaOrder.DeleteOrderItems(typeof(EnovaProductOrderItem));
            }
            else
            {
                //go through all rows and make sure they match
                foreach (var cartItem in currentCart.Rows)
                {
                    var enovaOrderItem = enovaOrder.GetOrderItems<EnovaProductOrderItem>()
                            .FirstOrDefault(x => x.ProductIdentifier == cartItem.Product);
                    var quantity = cartItem.Quantity > 0 ? cartItem.Quantity : 1;
                    if (enovaOrderItem != null && enovaOrderItem.OrderedQuantity != quantity)
                    {
                        enovaOrderItem.OrderedQuantity = quantity;
                    }
                    else if (enovaOrderItem == null)
                    {
                        enovaOrderItem = EnovaObjectCreationHelper.CreateNew<EnovaProductOrderItem>(context);
                        enovaOrderItem.Product = EnovaBaseProduct.Find(context, cartItem.Product);
                        enovaOrderItem.OrderedQuantity = quantity;
                        enovaOrder.AddOrderItem(enovaOrderItem);
                    }
                    
                    cartItem.AdditionalValues = _mappingToService.MapTo(enovaOrderItem, cartItem.AdditionalValues);
                    
                }
            }
        
        }
    }
}
