using System;
using System.Collections.Generic;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Abstractions.Interfaces;
using Wipcore.Enova.Api.Abstractions.Internal;
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
        public bool PostSaveSet => false;
        public bool FlattenMapping => false;

        public void SetEnovaProperty(BaseObject obj, string propertyName, object value, IDictionary<string, object> otherValues)
        {
            throw new NotImplementedException();
        }

        public object GetEnovaProperty(BaseObject obj, string propertyName, List<EnovaLanguage> mappingLanguages)
        {
            var address = (EnovaCustomerAddress) obj;
            return ((EnovaCustomerAddress.AddressTypeEnum) address.AddressType).ToString();
        }
    }
}
