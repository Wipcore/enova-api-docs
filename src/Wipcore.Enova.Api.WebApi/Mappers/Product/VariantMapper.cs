﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Wipcore.Core;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Abstractions.Interfaces;
using Wipcore.Enova.Api.Abstractions.Internal;
using Wipcore.Enova.Api.WebApi.Helpers;
using Wipcore.Enova.Core;
using Wipcore.Enova.Generics;

namespace Wipcore.Enova.Api.WebApi.Mappers.Product
{
    /// <summary>
    /// Maps connection to a variant family for a product - if the product is an owner or has an owner.
    /// </summary>
    public class VariantMapper : IPropertyMapper, ICmoProperty
    {
        public bool PostSaveSet => true;
        private readonly IProductService _productService;

        public VariantMapper(IProductService productService)
        {
            _productService = productService;
        }

        public List<string> Names => new List<string>() {"IsOwner", "VariantOwnerIdentifier", "VariantOwnerId", "VariantIds", "VariantIdentifiers"};
        public Type CmoType => typeof(CmoEnovaBaseProduct);
        public Type Type => typeof(EnovaBaseProduct);
        public bool InheritMapper => true;
        public bool FlattenMapping => false;
        public int Priority => 0;
        public MapType MapType => MapType.MapFromAndToEnovaAllowed;

        public void SetEnovaProperty(BaseObject obj, string propertyName, object value, IDictionary<string, object> otherValues)
        {
            if (value == null)
                return;

            if (String.Equals(propertyName, "IsOwner", StringComparison.InvariantCultureIgnoreCase))
                return;
            
            var product = (EnovaBaseProduct)obj;

            if (String.Equals(propertyName, "VariantIds", StringComparison.InvariantCultureIgnoreCase) || String.Equals(propertyName, "VariantIdentifiers", StringComparison.InvariantCultureIgnoreCase))
            {
                var variants = String.Equals(propertyName, "VariantIds", StringComparison.InvariantCultureIgnoreCase)
                    ? (IList)JsonConvert.DeserializeObject<List<int>>(value.ToString())
                    : (IList)JsonConvert.DeserializeObject<List<string>>(value.ToString());
                
                //should be owner if there are variant ids and no specified owner
                var shouldBeOwner = variants?.Count > 0 && otherValues.GetValueInsensitive<int>("VariantOwnerId") == default(int) &&
                    String.IsNullOrEmpty(Convert.ToString(otherValues.GetValueInsensitive<string>("VariantOwnerIdentifier")));

                if (!shouldBeOwner && product.IsVariantOwner) //if you shouldn't be an owner, but are, then stop it!
                {
                    var family = product.GetVariantFamily();
                    family.Delete();
                }

                if (shouldBeOwner)
                {
                    if (variants is List<int> variantIds)
                        _productService.SetupVariantFamily(product, variantIds);
                    else
                        _productService.SetupVariantFamily(product, (List<string>) variants);
                }
                return;
            }

            //if ownerid or owneridentifier is set, then this product should be a member of a family
            if (String.Equals(propertyName, "VariantOwnerId", StringComparison.InvariantCultureIgnoreCase))
            {
                var ownerId = Convert.ToInt32(value);
                if (ownerId == default (int))
                    return;
                _productService.SetProductAsVariant(product, ownerId: ownerId);
            }

            if (String.Equals(propertyName, "VariantOwnerIdentifier", StringComparison.InvariantCultureIgnoreCase))
            {
                var variantOwnerIdentifier = value?.ToString();
                if (String.IsNullOrEmpty(variantOwnerIdentifier))
                    return;
                _productService.SetProductAsVariant(product, ownerIdentifier: variantOwnerIdentifier);
            }
        }

        public object GetEnovaProperty(BaseObject obj, string propertyName, List<EnovaLanguage> mappingLanguages)
        {
            var product = (EnovaBaseProduct)obj;
            var family = product.GetVariantFamily(true, true);
            var owner = family?.Owner;
            var isOwner = obj.ID == owner?.ID;

            if (String.Equals(propertyName, "IsOwner", StringComparison.InvariantCultureIgnoreCase))
                return isOwner;
            if (String.Equals(propertyName, "VariantOwnerIdentifier", StringComparison.InvariantCultureIgnoreCase))
                return isOwner ? null : owner?.Identifier ?? "";
            if (String.Equals(propertyName, "VariantOwnerId", StringComparison.InvariantCultureIgnoreCase))
                return isOwner ? null : owner?.ID;
            if (String.Equals(propertyName, "VariantIds", StringComparison.InvariantCultureIgnoreCase))
                return product.GetVariantMembers().Where(x => x.ID != product.ID && (owner == null || x.ID != owner.ID)).Select(x => x.ID).ToList();
            if (String.Equals(propertyName, "VariantIdentifiers", StringComparison.InvariantCultureIgnoreCase))
                return product.GetVariantMembers().Where(x => x.Identifier != product.Identifier && (owner == null || x.Identifier != owner.Identifier)).Select(x => x.Identifier).ToList();

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
            if (String.Equals(propertyName, "VariantOwnerIdentifier", StringComparison.InvariantCultureIgnoreCase))
                return isOwner ? "" : owner?.GetIdentifier(context) ?? "";
            if (String.Equals(propertyName, "VariantOwnerId", StringComparison.InvariantCultureIgnoreCase))
                return isOwner ? 0 : owner?.ID;

            return null;
        }
    }
}
