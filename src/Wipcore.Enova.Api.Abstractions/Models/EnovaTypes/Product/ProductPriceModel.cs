using System.Collections.Generic;
using Wipcore.Enova.Api.Abstractions.Attributes;
using Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.Customer;

namespace Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.Product
{
    public class ProductPriceModel
    {
        [PropertyPresentation("String", "Pricelist Id", isEditable: false, isFilterable: true, isGridColumn: false, sortOrder: 15000)]
        public int PriceListId { get; set; }

        [PropertyPresentation("String", "Pricelist identifier", isEditable: false, isFilterable: true, isGridColumn: false, sortOrder: 15100)]
        public string PriceListIdentifier { get; set; }

        [PropertyPresentation("NumberString", "Price on pricelist excl tax", isEditable: false, isFilterable: true, isGridColumn: false, sortOrder: 15200)]
        public decimal PriceExclTax { get; set; }

        [PropertyPresentation("NumberString", "Price on pricelist incl tax", isEditable: false, isFilterable: true, isGridColumn: false, sortOrder: 15200)]
        public decimal PriceInclTax { get; set; }

        [PropertyPresentation("Currency", "Pricelist currency", isEditable: false, isFilterable: true, isGridColumn: false, sortOrder: 15200)]
        public string Currency { get; set; }

        public List<CustomerGroupMiniModel> GroupsWithAccessToPrice { get; set; }
    }
}
