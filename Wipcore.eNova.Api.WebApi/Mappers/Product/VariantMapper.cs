using System;
using System.Collections;
using System.Collections.Generic;
using Wipcore.Core;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Interfaces;
using Wipcore.Enova.Core;

namespace Wipcore.eNova.Api.WebApi.Mappers.Product
{
    public class VariantMapper : IPropertyMapper, ICmoProperty
    {
        public List<string> Names => new List<string>() {"IsOwner", "VariantOwner"};
        public Type CmoType => typeof(CmoEnovaBaseProduct);
        public Type Type => typeof(EnovaBaseProduct);
        public bool InheritMapper => true;
        public int Priority => 0;
        public MapType MapType => MapType.MapFrom;

        public object MapToEnovaProperty(BaseObject obj, string propertyName)
        {
            throw new NotImplementedException();
        }

        public object MapFromEnovaProperty(BaseObject obj, string propertyName)
        {
            var product = (EnovaBaseProduct)obj;
            var family = product.GetVariantFamily(true, true);
            var owner = family?.Owner;
            var isOwner = obj.ID == owner?.ID;

            if (String.Equals(propertyName, "IsOwner", StringComparison.InvariantCultureIgnoreCase))
                return isOwner;
            if (String.Equals(propertyName, "VariantOwner", StringComparison.InvariantCultureIgnoreCase))
                return isOwner ? null : owner?.Identifier ?? "";

            return null;
        }

        public object GetProperty(CmoDbObject obj, CmoContext context, string propertyName, CmoLanguage language)
        {
            var product = (CmoEnovaBaseProduct)obj;
            var families = new ArrayList();
            product.GetFamilies(context, families, true, true, typeof (CmoVariantFamily));
            var family = families.Count > 0 ?  families[0] as CmoVariantFamily : null;

            var owners = new ArrayList(); ;
            family?.GetOwners(context, owners, null);
            var owner = owners.Count > 0 ?  owners[0] as CmoObject : null;
            var isOwner = product.ID == owner?.ID;

            if (String.Equals(propertyName, "IsOwner", StringComparison.InvariantCultureIgnoreCase))
                return isOwner;
            if (String.Equals(propertyName, "VariantOwner", StringComparison.InvariantCultureIgnoreCase))
                return isOwner ? "" : owner?.GetIdentifier(context) ?? "";

            return null;
        }
    }
}
