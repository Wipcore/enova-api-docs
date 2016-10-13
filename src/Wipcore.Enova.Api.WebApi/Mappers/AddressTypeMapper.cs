using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Interfaces;
using Wipcore.Enova.Core;

namespace Wipcore.Enova.Api.WebApi.Mappers
{
    /// <summary>
    /// Maps addresstype enum for a customer address.
    /// </summary>
    public class AddressTypeMapper : IPropertyMapper
    {
        public List<string> Names => new List<string>() { "AddressType" };
        public Type Type => typeof (EnovaCustomerAddress);
        public bool InheritMapper => true;
        public int Priority => 0;
        public MapType MapType => MapType.MapFromEnovaAllowed;

        public void MapToEnovaProperty(BaseObject obj, string propertyName, object value, IDictionary<string, object> otherValues)
        {
            throw new NotImplementedException();
        }

        public object MapFromEnovaProperty(BaseObject obj, string propertyName)
        {
            var address = (EnovaCustomerAddress) obj;
            return ((EnovaCustomerAddress.AddressTypeEnum) address.AddressType).ToString();
        }
    }
}
