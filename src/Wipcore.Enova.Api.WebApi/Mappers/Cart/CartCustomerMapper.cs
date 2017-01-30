using System;
using System.Collections.Generic;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Abstractions.Interfaces;
using Wipcore.Enova.Core;

namespace Wipcore.eNova.Api.WebApi.Mappers.Cart
{
    public class CartCustomerMapper : IPropertyMapper
    {
        public List<string> Names => new List<string>() { "CustomerID", "CustomerIdentifier" };
        public Type Type => typeof(EnovaCart);
        public bool InheritMapper => true;
        public int Priority => 0;
        public MapType MapType => MapType.MapFromAndToEnovaAllowed;
        public bool PostSaveSet => false;
        public bool FlattenMapping => false;
        public object GetEnovaProperty(BaseObject obj, string propertyName)
        {
            var cart = (EnovaCart)obj;

            if (propertyName.Equals("CustomerID", StringComparison.InvariantCultureIgnoreCase))
                return cart.Customer?.ID ?? 0;

            return cart.Customer?.Identifier;
        }

        public void SetEnovaProperty(BaseObject obj, string propertyName, object value, IDictionary<string, object> otherValues)
        {
            var cart = (EnovaCart)obj;
            var context = cart.GetContext();

            if (value == null)
            {
                cart.Customer = null;
                return;
            }

            if (propertyName.Equals("CustomerID", StringComparison.InvariantCultureIgnoreCase))
            {
                var id = Convert.ToInt32(value);
                if (id == 0)
                    cart.Customer = null;
                else if(cart.Customer?.ID != id)
                {
                    cart.Customer = EnovaCustomer.Find(context, id);
                }
            }
            else
            {
                var identifier = value.ToString();
                if (String.IsNullOrEmpty(identifier))
                    cart.Customer = null;
                else if (cart.Customer?.Identifier != identifier)
                {
                    cart.Customer = EnovaCustomer.Find(context, identifier);
                }
            }
          
        }
    }

}
