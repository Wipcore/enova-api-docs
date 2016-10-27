using System;
using System.Collections.Generic;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Interfaces;
using Wipcore.Enova.Core;

namespace Wipcore.eNova.Api.WebApi.Mappers.PriceList
{
    public class PriceListCurrencyMapper : IPropertyMapper
    {
        public List<string> Names => new List<string>() { "Currency"};
        public Type Type => typeof(EnovaPriceList);
        public bool InheritMapper => true;
        public int Priority => 0;
        public MapType MapType => MapType.MapFromAndToEnovaAllowed;


        public object GetEnovaProperty(BaseObject obj, string propertyName)
        {
            var pricelist = (EnovaPriceList) obj;
            return pricelist.Currency?.Identifier;
        }

        public void SetEnovaProperty(BaseObject obj, string propertyName, object value, IDictionary<string, object> otherValues)
        {
            var pricelist = (EnovaPriceList)obj;
            pricelist.Currency = EnovaCurrency.Find(pricelist.GetContext(), value.ToString());
        }
    }
}
