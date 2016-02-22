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
using static System.String;

namespace Wipcore.Enova.Api.WebApi.Services
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

            var enovaCartItems = enovaCart.GetCartItems<EnovaProductCartItem>().ToList();
            foreach (var cartRow in currentCart.Rows) //fill in price information
            {
                var item = enovaCartItems.First(x => x.Product.Identifier == cartRow.Product);
                cartRow.PriceExclTax = item.GetPriceExclTax();
                cartRow.PriceInclTax = item.GetPriceInclTax();
            }

            var currency = context.CurrentCurrency;
            decimal rounding, taxAmount;
            int decimals;
            var totalPrice = enovaCart.GetPrice(out taxAmount, out rounding, out decimals, ref currency);

            currentCart.TotalPriceExclTax = totalPrice;
            currentCart.TotalPriceInclTax = totalPrice + taxAmount;

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

            if (currentCart.AdditionalValues != null)
            {
                currentCart.AdditionalValues = _mappingToService.MapTo(enovaCart, currentCart.AdditionalValues);
            }

            //if no rows in given cart, make sure no rows in enova cart
            if (currentCart.Rows == null || !currentCart.Rows.Any())
            {
                enovaCart.DeleteCartItems(typeof(EnovaProductCartItem));
            }
            else
            {
                //go through all rows and make sure they match
                foreach (var cartItem in currentCart.Rows)
                {
                    var product = EnovaBaseProduct.Find(context, cartItem.Product);
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

                    if (cartItem.AdditionalValues != null)
                    {
                        cartItem.AdditionalValues = _mappingToService.MapTo(enovaCartItem, cartItem.AdditionalValues);
                    }
                }
            }
        }
    }
}
