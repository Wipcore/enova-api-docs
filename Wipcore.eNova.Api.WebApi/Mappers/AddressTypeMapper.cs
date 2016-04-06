using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Interfaces;
using Wipcore.Enova.Core;

namespace Wipcore.eNova.Api.WebApi.Mappers
{
    public class AddressTypeMapper : IPropertyMapper
    {
        public List<string> Names => new List<string>() { "AddressType" };
        public Type Type => typeof (EnovaCustomerAddress);
        public bool InheritMapper => true;
        public int Priority => 0;
        public MapType MapType => MapType.MapFrom;

        public object MapTo(BaseObject obj, string propertyName)
        {
            throw new NotImplementedException();
        }

        public object MapFrom(BaseObject obj, string propertyName)
        {
            var address = (EnovaCustomerAddress) obj;
            return ((EnovaCustomerAddress.AddressTypeEnum) address.AddressType).ToString();
        }
    }
}
