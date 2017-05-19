using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Wipcore.Core;
using Wipcore.Core.SessionObjects;
using Wipcore.eNova.Api.WebApi.Mappers;
using Wipcore.Enova.Api.Abstractions.Interfaces;
using Wipcore.Enova.Api.WebApi.Helpers;
using Wipcore.Enova.Core;
using Wipcore.Enova.Generics;

namespace Wipcore.Enova.Api.WebApi.Mappers.Product
{
    /// <summary>
    /// Maps connection to a variant family for a product - if the product is an owner or has an owner.
    /// </summary>
    public class VariantModelMapper : IPropertyMapper
    {
        public bool PostSaveSet => true;
        private readonly IProductService _productService;

        public VariantModelMapper(IProductService productService)
        {
            _productService = productService;
        }

        public List<string> Names => new List<string>() { "Variants", "VariantOwner" };
        public Type Type => typeof(EnovaBaseProduct);
        public bool InheritMapper => true;
        public bool FlattenMapping => false;
        public int Priority => 0;
        public MapType MapType => MapType.MapFromAndToEnovaAllowed;

        public object GetEnovaProperty(BaseObject obj, string propertyName, List<EnovaLanguage> mappingLanguages)
        {
            var product = (EnovaBaseProduct)obj;

            if (String.Equals(propertyName, "Variants", StringComparison.InvariantCultureIgnoreCase))
            {
                return product.GetVariantMembers().Where(x => x.ID != product.ID).Select(x => new Dictionary<string, object>()
                {
                    {"ID", x.ID},
                    {"Identifier", x.Identifier}
                }.MapLanguageProperty("Name", mappingLanguages, x.GetName)
                 .MapLanguageProperty("Attributes", mappingLanguages, language => x.GetAttributeValues().Cast<EnovaAttributeValue>()
                    .Select(a => $"{a.AttributeType?.Identifier ?? ""} : {(!String.IsNullOrEmpty(a.ValueCode) ? a.ValueCode : a.GetName(language))}")));
            }

            var owner = product.GetVariantOwner();
            if (owner == null || owner.ID == product.ID)
                return null;

            var ownerAttributes = owner.GetAttributeValues().Cast<EnovaAttributeValue>();
            return new Dictionary<string, object>()
            {
                {"ID", owner.ID},
                { "Identifier", owner.Identifier}
            }.MapLanguageProperty("Name", mappingLanguages, owner.GetName)
             .MapLanguageProperty("Attributes", mappingLanguages, language => 
                ownerAttributes.Select(a => $"{a.AttributeType?.Identifier ?? ""} : {(!String.IsNullOrEmpty(a.ValueCode) ? a.ValueCode : a.GetName(language))}"));

        }

        public void SetEnovaProperty(BaseObject obj, string propertyName, object value, IDictionary<string, object> otherValues)
        {
            if (value == null)
                return;

            var product = (EnovaBaseProduct)obj;
            var context = product.GetContext();
            var variants = new List<EnovaBaseProduct>();

            if (String.Equals(propertyName, "Variants", StringComparison.InvariantCultureIgnoreCase))
            {
                foreach (var i in value as dynamic)
                {
                    var variantModel = JsonConvert.DeserializeAnonymousType(i.ToString(), new { ID = 0, Identifier = String.Empty, MarkForDelete = false });
                    if(variantModel.MarkForDelete)
                        continue;

                    var variant = context.FindObject(variantModel.ID, typeof(EnovaBaseProduct), false) ??
                                  context.FindObject(variantModel.Identifier, typeof(EnovaBaseProduct), true);
                    variants.Add((EnovaBaseProduct)variant);
                }

                //should be owner if there are variants and no specified owner
                var shouldBeOwner = variants.Count > 0 && otherValues.GetValueInsensitive<object>("VariantOwner") == null;

                if (!shouldBeOwner && product.IsVariantOwner) //if you shouldn't be an owner, but are, then stop it!
                {
                    var family = product.GetVariantFamily();
                    family.Delete();
                }

                if (shouldBeOwner)
                {
                    _productService.SetupVariantFamily(product, variants);
                }
                return;
            }

            //if owner is set, then this product should be a member of a family
            if (String.Equals(propertyName, "VariantOwner", StringComparison.InvariantCultureIgnoreCase))
            {
                var ownerModel = JsonConvert.DeserializeAnonymousType(value.ToString(), new { ID = 0, Identifier = String.Empty });
                
                _productService.SetProductAsVariant(product, ownerModel.ID, ownerModel.Identifier);
            }


        }

        

        
    }
}
