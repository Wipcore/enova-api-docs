using System.Collections.Generic;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Core;

namespace Wipcore.Enova.Api.Abstractions.Internal
{
    public interface IProductService
    {
        /// <summary>
        /// Get all members of any variant family the given owner is a member of. Null if no family exists.
        /// </summary>
        BaseObjectList GetVariants(string identifier);


        void SetProductAsVariant(EnovaBaseProduct product, int ownerId = 0, string ownerIdentifier = null);
        void SetupVariantFamily(EnovaBaseProduct owner, List<int> variantIds);

        void SetupVariantFamily(EnovaBaseProduct owner, List<string> variantIdentifiers);

        void SetupVariantFamily(EnovaBaseProduct owner, List<EnovaBaseProduct> variants);
    }
}