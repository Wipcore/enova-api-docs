using System;
using System.Collections.Generic;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Interfaces;
using Wipcore.Enova.Core;

namespace Wipcore.eNova.Api.WebApi.Mappers.Product
{
    public class VariantMapper : IPropertyMapper
    {
        public List<string> Names => new List<string>() {"IsOwner", "VariantOwner"};
        public Type Type => typeof(EnovaBaseProduct);
        public bool InheritMapper => true;
        public int Priority => 0;
        public MapType MapType => MapType.MapFrom;

        public object MapTo(BaseObject obj, string propertyName)
        {
            throw new NotImplementedException();
        }

        public object MapFrom(BaseObject obj, string propertyName)
        {
            var product = (EnovaBaseProduct)obj;
            var family = product.GetVariantFamily(true, true);
            var owner = family?.Owner;
            var isOwner = obj.ID == owner?.ID;

            if (String.Equals(propertyName, "IsOwner", StringComparison.InvariantCultureIgnoreCase))
                return isOwner;
            if (String.Equals(propertyName, "VariantOwner", StringComparison.InvariantCultureIgnoreCase))
                return isOwner ? null : owner?.Identifier;

            return null;
        }
    }
}
