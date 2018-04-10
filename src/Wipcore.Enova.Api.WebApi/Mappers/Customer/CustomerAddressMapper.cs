using System;
using System.Collections.Generic;
using System.Linq;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Abstractions.Internal;
using Wipcore.Enova.Core;

namespace Wipcore.Enova.Api.WebApi.Mappers.Customer
{
    public class CustomerAddressMapper : IPropertyMapper
    {
        public List<string> Names => new List<string>() { "CustomerAddresses" };
        public Type Type => typeof(EnovaCustomer);
        public bool InheritMapper => true;
        public int Priority => 0;
        public MapType MapType => MapType.MapFromEnovaAllowed;
        public bool PostSaveSet => false;
        public bool FlattenMapping => false;
        public object GetEnovaProperty(BaseObject obj, string propertyName, List<EnovaLanguage> languages)
        {
            var customer = (EnovaCustomer)obj;
            var addresses = new List<object>();
            foreach (var address in customer.GetAddresses(typeof(EnovaCustomerAddress)).OfType<EnovaCustomerAddress>())
            {
                var miniAddress = new Dictionary<string, object>
                {
                    {"Id", address.ID },
                    {"Identifier", address.Identifier},
                    {"Street", address.Street },
                    {"PostalCode", address.PostalCode },
                    {"City", address.City },
                    {"Country", address.CountryName },
                    {"Phone", address.Phone },
                    {"AddressType", ((EnovaCustomerAddress.AddressTypeEnum)address.AddressType).ToString() }
                };
                addresses.Add(miniAddress);
            }
            return addresses;
        }

        public void SetEnovaProperty(BaseObject obj, string propertyName, object value, IDictionary<string, object> otherValues)
        {
            throw new NotImplementedException();
        }
    }
}
