using System.Collections.Generic;
using Wipcore.Core.SessionObjects;
using Wipcore.eNova.Api.WebApi.Services;
using Wipcore.Enova.Api.Interfaces;
using Wipcore.Enova.Core;
using Wipcore.Enova.Generics;
using System.Linq;

namespace Wipcore.eNova.Api.WebApi.EnovaObjectServices
{
    public class ProductService : IProductService
    {
        private readonly IContextService _contextService;

        public ProductService(IContextService contextService)
        {
            _contextService = contextService;
        }

        /// <summary>
        /// Get all members of any variant family the given owner is a member of. Null if no family exists.
        /// </summary>
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

        public void SetProductAsVariant(EnovaBaseProduct product, int ownerId = 0, string ownerIdentifier = null)
        {
            var context = _contextService.GetContext();
            var owner = context.FindObject<EnovaBaseProduct>(ownerId) ??
                        context.FindObject<EnovaBaseProduct>(ownerIdentifier);

            //if owner null, mabe throw exception TODO
            var ownerFamily = GetOrCreateVariantFamily(owner);
            ownerFamily.AddObject(product);
        }

        public void SetupVariantFamily(EnovaBaseProduct owner, List<int> variantIds)
        {
            var context = _contextService.GetContext();
            var ownerFamily = GetOrCreateVariantFamily(owner);
            var currentVariantMembers = ownerFamily.GetFamilyMembers<EnovaBaseProduct>().Where(x => x.ID != owner.ID).ToList();
            var newVariants = variantIds.Distinct().Except(currentVariantMembers.Select(x => x.ID)).Select(x => context.FindObject<EnovaBaseProduct>(x)).Where(x => x != null).ToList();
            var deletedVariants = currentVariantMembers.Where(x => !variantIds.Contains(x.ID)).ToList();

            newVariants.ForEach(x => ownerFamily.AddObject(x));
            deletedVariants.ForEach(x => ownerFamily.RemoveObject(x));
        }

        private VariantFamily GetOrCreateVariantFamily(EnovaBaseProduct variantOwner)
        {
            var context = _contextService.GetContext();
            var ownerfamily = variantOwner.GetVariantFamily();

            if (ownerfamily == null)
            {
                var ownerFamilyIdentifier = variantOwner.Identifier + "_Family";
                ownerfamily = context.FindObject<EnovaVariantFamily>(ownerFamilyIdentifier);
                if (ownerfamily != null) //found a family that is not connected
                {
                    ownerfamily.Owner = variantOwner;
                }
                else
                {
                    ownerfamily = new EnovaVariantFamily(context) { Identifier = ownerFamilyIdentifier };
                    ownerfamily.Save();

                    ownerfamily.Owner = variantOwner;
                }
            }
            return ownerfamily;
        }
    }
}
