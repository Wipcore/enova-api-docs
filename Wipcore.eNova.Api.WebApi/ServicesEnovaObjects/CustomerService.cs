using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Interfaces;
using Wipcore.Enova.Core;
using Wipcore.Enova.Generics;

namespace Wipcore.eNova.Api.WebApi.ServicesEnovaObjects
{
    public class CustomerService : ICustomerService
    {
        private readonly IContextService _contextService;

        public CustomerService(IContextService contextService)
        {
            _contextService = contextService;
        }


        public BaseObjectList GetAddresses(string customerIdentifier, EnovaCustomerAddress.AddressTypeEnum? addressType = null)
        {
            var context = _contextService.GetContext();
            var customer = EnovaCustomer.Find(context, customerIdentifier);

            var searchExpression = "AddressableID =" + customer.ID;
            if (addressType.HasValue)
                searchExpression += " AND AddressType = " + (int)addressType.Value;

            var addresses = context.Search(searchExpression, typeof(EnovaCustomerAddress), null, 0, null, false);
            return addresses;
        }
    }
}
