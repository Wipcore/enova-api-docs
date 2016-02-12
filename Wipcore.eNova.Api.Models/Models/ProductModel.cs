using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wipcore.Enova.Api.WebApi.Models
{
    public class ProductModel : BaseModel
    {
        public List<LocalizedString> Name { get; set; }

        /// <summary>
        /// Identifier of this products default section 
        /// </summary>
        public string DefaultSection { get; set; }

        public List<LocalizedString> DescriptionLong { get; set; }

        public List<LocalizedString> DescriptionShort { get; set; }
        
        public decimal Price { get; set; }
        /// <summary>
        /// Identifier of this products mainprice tax
        /// </summary>
        public string MainPriceTax { get; set; }
        
        //public List<LocalizedString> Manufacturer { get; set; }
        public string Manufacturer { get; set; }
        
        /// <summary>
        /// Identifier of this products parent section.
        /// </summary>
        /// <remarks>
        /// If the product belongs to more than one section, this property will return the first parent. Setting this property will delete the connections to all other existing parents.
        /// </remarks>
        public string Parent { get; set; }
        
        /// <summary>
        /// Value of Wipcore.Core.SessionObjects.Product.PriceDisplayRuleType Enumeration
        /// </summary>
        public int PriceDisplayRule { get; set; }
        
        /// <summary>
         /// Identifier of this products product group
        /// </summary>
        public string ProductGroup { get; set; }
        
        /// <summary>
        /// Value of Wipcore.Core.SessionObjects.Product.QuantityValidationRuleType Enumeration
        /// </summary>
        public int QuantityValidationRule { get; set; }
        
        /// <summary>
        /// Identifier of this products unit
        /// </summary>
        public string Unit { get; set; }

        public double Weight { get; set; }

        //public List<AttributeTypeModel> AttributeTypes { get; set; }
        //public List<ProductModel> Variants { get; set; }
        //public List<AttributeTypeGroupModel> AttributeTypeGroups { get; set; }
        ///// <summary>
        ///// List of identifiers of the sections this product is connected to.
        ///// </summary>
        //public List<string> Sections { get; set; }

        //// Related Products

        ///// <summary>
        ///// Relations from OwnerProduct to consumption products
        ///// </summary>
        //public List<ProductOwnerRelation> ConsumptionProducts { get; set; }

        ///// <summary>
        ///// Relations from OwnerProduct to "up sell products"
        ///// </summary>
        //public List<ProductOwnerRelation> UpSellRelations { get; set; }

        ///// <summary>
        ///// Relations from OwnerProduct to Accessory products
        ///// </summary>
        //public List<ProductOwnerRelation> AccessoryRelations { get; set; }

    }
}
