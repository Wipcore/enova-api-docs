using System.Collections.Generic;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Models;
using Wipcore.Enova.Core;

namespace Wipcore.Enova.Api.Interfaces
{
    public interface ICustomerService
    {
        /// <summary>
        /// Saves a customer with given values.
        /// </summary>
        IDictionary<string, object> SaveCustomer(ContextModel requestContext, Dictionary<string, object> values);

        /// <summary>
        /// Get addresses belonging to given customer.
        /// </summary>
        BaseObjectList GetAddresses(string customerIdentifier, EnovaCustomerAddress.AddressTypeEnum? addressType = null);
    }
}