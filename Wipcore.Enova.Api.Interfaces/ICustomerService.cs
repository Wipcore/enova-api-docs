using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Core;

namespace Wipcore.Enova.Api.Interfaces
{
    public interface ICustomerService
    {
        BaseObjectList GetAddresses(string customerIdentifier, EnovaCustomerAddress.AddressTypeEnum? addressType = null);
    }
}