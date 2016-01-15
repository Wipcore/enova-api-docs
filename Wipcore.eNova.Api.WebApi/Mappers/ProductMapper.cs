using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Wipcore.Core;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Core;
using Wipcore.Enova.Generics;
using Wipcore.eNova.Api.WebApi.Mappers;
using Wipcore.eNova.Api.WebApi.Models;

namespace Wipcore.eNova.Api.WebApi.Mappers
{
    public class ProductMapper : LanguageMapper<EnovaBaseProduct, ProductModel>
    {
        //protected readonly EnovaAttributeTypeMapper _attributeTypeMapper;
        //protected readonly EnovaAttributeValueMapper _attributeValueMapper;
        //protected readonly EnovaAttributeTypeGroupMapper _attributeTypeGroupMapper;

        //public EnovaProductMapper(Func<ProductModel> modelMaker, Func<Context, EnovaBaseProduct> enovaMaker, 
        //    EnovaAttributeTypeMapper attributeTypeMapper, EnovaAttributeValueMapper attributeValueMapper, EnovaAttributeTypeGroupMapper attributeTypeGroupMapper)
        //    : base(modelMaker, enovaMaker)
        //{
        //    _attributeTypeMapper = attributeTypeMapper;
        //    _attributeValueMapper = attributeValueMapper;
        //    _attributeTypeGroupMapper = attributeTypeGroupMapper;
        //}

        public ProductMapper(Func<ProductModel> modelMaker, Func<Context, EnovaBaseProduct> enovaMaker)
            : base(modelMaker, enovaMaker)
        {
        }

        public override EnovaBaseProduct MapToEnova(ProductModel model)
        {
            //var product = base.MapToEnova(model, false);
            var product = base.MapToEnova(model);

            //DefaultSection
            if (!String.IsNullOrEmpty(model.DefaultSection))
            {
                var section = Context.FindObject<EnovaBaseProductSection>(model.DefaultSection);
                if (section != null)
                    product.DefaultSection = section;
            }

            //MainPriceTax
            if (!String.IsNullOrEmpty(model.MainPriceTax))
            {
                var tax = Context.FindObject<Tax>(model.MainPriceTax);
                if (tax != null)
                    product.MainPriceTax = tax;
            }

            //Manufacturer
            if (!String.IsNullOrEmpty(model.Manufacturer))
            {
                var manufacturer = Context.FindObject<Manufacturer>(model.Manufacturer);
                if (manufacturer != null)
                    product.Manufacturer = manufacturer;
            }

            //Parent
            if (!String.IsNullOrEmpty(model.Parent))
            {
                var parent = Context.FindObject<EnovaBaseProductSection>(model.Parent);
                if (parent != null)
                    product.Parent = parent;
            }

            //PriceDisplayRule // TODO: test
            if (model.PriceDisplayRule != null)
            {
                product.PriceDisplayRule = (Product.PriceDisplayRuleType)model.PriceDisplayRule;
            }

            //ProductGroup
            if (!String.IsNullOrEmpty(model.ProductGroup))
            {
                var productGroup = Context.FindObject<EnovaProductGroup>(model.ProductGroup);
                if (productGroup != null)
                    product.ProductGroup = productGroup;
            }

            //QuantityValidationRule // TODO: test
            if (model.QuantityValidationRule != null)
            {
                product.QuantityValidationRule = (Product.QuantityValidationRuleType)model.QuantityValidationRule;
            }

            //Unit
            if (!String.IsNullOrEmpty(model.Unit))
            {
                var unit = Context.FindObject<Unit>(model.Unit);
                if (unit != null)
                    product.Unit = unit;
            }
            
            return product;
        }

        public override ProductModel MapFromEnova(EnovaBaseProduct product)
        {
            throw new NotImplementedException();
            //TODO: not complete
            //ProductModel model = new ProductModel();
            //model.Identifier = product.Identifier;
            //model.Name = new List<LocalizedString>();
            //model.Name.Add(new LocalizedString { Culture = "en-GB", Value = product.Name });
            //return model;
        }
    }
}