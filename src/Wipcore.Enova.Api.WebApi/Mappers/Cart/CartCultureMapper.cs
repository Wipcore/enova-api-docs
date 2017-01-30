using System;
using System.Collections.Generic;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Abstractions.Interfaces;
using Wipcore.Enova.Core;

namespace Wipcore.eNova.Api.WebApi.Mappers.Cart
{
    public class CartCultureMapper : IPropertyMapper
    {
        public List<string> Names => new List<string>() { "Currency"};
        public Type Type => typeof(EnovaCart);
        public bool InheritMapper => true;
        public int Priority => 0;
        public MapType MapType => MapType.MapFromAndToEnovaAllowed;
        public bool PostSaveSet => false;
        public bool FlattenMapping => false;

        public object GetEnovaProperty(BaseObject obj, string propertyName)
        {
            var cart = (EnovaCart) obj;
            return cart.Currency?.Identifier;
        }

        public void SetEnovaProperty(BaseObject obj, string propertyName, object value, IDictionary<string, object> otherValues)
        {
            var cart = (EnovaCart)obj;
            cart.Currency = value == null ? null : EnovaCurrency.Find(cart.GetContext(), value.ToString());
        }
    }
}
