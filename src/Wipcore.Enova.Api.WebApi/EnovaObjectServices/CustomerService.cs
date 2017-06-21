using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Abstractions.Interfaces;
using Wipcore.Enova.Api.Abstractions.Internal;
using Wipcore.Enova.Api.Abstractions.Models;
using Wipcore.Enova.Api.WebApi.Helpers;
using Wipcore.Enova.Core;

namespace Wipcore.Enova.Api.WebApi.EnovaObjectServices
{
    public class CustomerService : ICustomerService
    {
        private readonly IContextService _contextService;
        private readonly IAuthService _authService;
        private readonly IObjectService _objectService;

        public CustomerService(IContextService contextService, IAuthService authService, IObjectService objectService)
        {
            _contextService = contextService;
            _authService = authService;
            _objectService = objectService;
        }

        /// <summary>
        /// Get addresses belonging to given customer.
        /// </summary>
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

        /// <summary>
        /// Saves a customer with given values.
        /// </summary>
        public IDictionary<string, object> SaveCustomer(ContextModel requestContext, Dictionary<string, object> values)
        {
            var identifier = values.FirstOrDefault(x => x.Key.Equals("identifier", StringComparison.CurrentCultureIgnoreCase)).Value?.ToString();
            if (String.IsNullOrEmpty(identifier))//if no identifier, make one!
            {
                identifier = EnovaCommonFunctions.GetSequenceNumber(_contextService.GetContext(), SystemRunningMode.Remote, SequenceType.CustomerIdentifier);
                values["Identifier"] = identifier;
            }
            else if (!_authService.AuthorizeUpdate(identifier, null))//otherwise make sure the customer is not updating another customer...
                throw new HttpException(HttpStatusCode.Unauthorized, "A customer can only update itself.");

            return _objectService.Save<EnovaCustomer>(requestContext, values);
        }
    }
}
