using System;
using System.Collections.Generic;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Abstractions.Interfaces;
using Wipcore.Enova.Api.Abstractions.Internal;
using Wipcore.Enova.Core;

namespace Wipcore.eNova.Api.WebApi.Mappers.Customer
{
    public class CustomerTaxMapper : IPropertyMapper
    {
        public List<string> Names => new List<string>() { "Taxrule" };
        public Type Type => typeof(EnovaCustomer);
        public bool InheritMapper => true;
        public int Priority => 0;
        public MapType MapType => MapType.MapFromAndToEnovaAllowed;

        public bool PostSaveSet => false;
        public bool FlattenMapping => false;
        public object GetEnovaProperty(BaseObject obj, string propertyName, List<EnovaLanguage> mappingLanguages)
        {
            var customer = (EnovaCustomer) obj;
            return customer.TaxationRule?.Identifier;
        }

        public void SetEnovaProperty(BaseObject obj, string propertyName, object value, IDictionary<string, object> otherValues)
        {
            if (value == null)
                return;

            var customer = (EnovaCustomer)obj;
            customer.TaxationRule = EnovaTaxationRule.Find(customer.GetContext(), value.ToString());
        }
    }
}
