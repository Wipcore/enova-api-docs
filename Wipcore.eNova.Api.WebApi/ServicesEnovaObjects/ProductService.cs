using Wipcore.Core.SessionObjects;
using Wipcore.eNova.Api.WebApi.Services;
using Wipcore.Enova.Api.Interfaces;
using Wipcore.Enova.Core;

namespace Wipcore.eNova.Api.WebApi.ServicesEnovaObjects
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
