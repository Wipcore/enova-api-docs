using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Abstractions.Internal;
using Wipcore.Enova.Core;
using Wipcore.Enova.Generics;

namespace Wipcore.Enova.Api.WebApi.Mappers.Customer
{
    public class CustomerCartsMapper : IPropertyMapper
    {
        public List<string> Names => new List<string>() { "CustomerCarts" };
        public Type Type => typeof(EnovaCustomer);
        public bool InheritMapper => true;
        public int Priority => 0;
        public MapType MapType => MapType.MapFromEnovaAllowed;
        public bool PostSaveSet => false;
        public bool FlattenMapping => false;
        public object GetEnovaProperty(BaseObject obj, string propertyName, List<EnovaLanguage> languages)
        {
            var customer = (EnovaCustomer)obj;
            var carts = new List<object>();

            foreach (var cart in customer.GetCarts(typeof(EnovaCart)).Cast<EnovaCart>()) 
            {
                var cartProducts = new List<object>();
                foreach (var product in cart.GetCartItems<EnovaProductCartItem>())
                {
                    var miniCartProduct = new Dictionary<string, object>
                    {
                        { "ID", product.ID },
                        { "Identifier", product.Product.Identifier },
                        { "Name", product.Product.Name },
                        { "Price", product.Product.MainPrice }
                    };
                    cartProducts.Add(miniCartProduct);
                }

                var miniCart = new Dictionary<string, object>()
                {
                    {"Id", cart.ID },
                    {"Identifier", cart.Identifier},
                    {"CreatedAt", cart.CreatedAt },
                    {"Products", cartProducts }
                };
                carts.Add(miniCart);
            }
            return carts;
        }

        public void SetEnovaProperty(BaseObject obj, string propertyName, object value, IDictionary<string, object> otherValues)
        {
            throw new NotImplementedException();
        }
    }
}
