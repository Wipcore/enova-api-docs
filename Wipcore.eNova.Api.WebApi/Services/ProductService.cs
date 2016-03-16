using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Interfaces;
using Wipcore.Enova.Core;
using Wipcore.Enova.Generics;

namespace Wipcore.eNova.Api.WebApi.Services
{
    public class ProductService : IProductService
    {
        private readonly IContextService _contextService;

        public ProductService(IContextService contextService)
        {
            _contextService = contextService;
        }

        public BaseObjectList GetVariants(string identifier)
        {
            var context = _contextService.GetContext();
            var product = EnovaBaseProduct.Find(context, identifier);
            var family = product.GetVariantFamily(true, true);
            var members = family?.GetObjects(typeof (EnovaBaseProduct), true);
            var owner = family?.Owner;

            members?.Add(owner);

            return members;
        }
    }
}
