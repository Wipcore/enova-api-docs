﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Interfaces;
using Wipcore.Enova.Core;

namespace Wipcore.eNova.Api.WebApi.Mappers.Customer
{
    public class CustomerCultureMapper : IPropertyMapper
    {
        public List<string> Names => new List<string>() { "Taxrule", "Currency", "Country" };
        public Type Type => typeof(EnovaCustomer);
        public bool InheritMapper => true;
        public int Priority => 0;
        public MapType MapType => MapType.MapFromAndToEnovaAllowed;


        public object MapFromEnovaProperty(BaseObject obj, string propertyName)
        {
            var customer = (EnovaCustomer) obj;
            if (propertyName.Equals("Taxrule", StringComparison.InvariantCultureIgnoreCase))
                return customer.TaxationRule?.Identifier;
            if (propertyName.Equals("Currency", StringComparison.InvariantCultureIgnoreCase))
                return customer.Currency?.Identifier;

            return customer.Country?.Identifier;
        }

        public void MapToEnovaProperty(BaseObject obj, string propertyName, object value, IDictionary<string, object> otherValues)
        {
            var customer = (EnovaCustomer)obj;
            if(propertyName.Equals("Taxrule", StringComparison.InvariantCultureIgnoreCase))
                customer.TaxationRule = EnovaTaxationRule.Find(customer.GetContext(), value.ToString());
            else if (propertyName.Equals("Currency", StringComparison.InvariantCultureIgnoreCase))
                customer.Currency = EnovaCurrency.Find(customer.GetContext(), value.ToString());
            else
                customer.Country = EnovaCountry.Find(customer.GetContext(), value.ToString());
        }
    }
}
