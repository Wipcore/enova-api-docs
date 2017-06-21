using Wipcore.Enova.Api.Abstractions.Attributes;

namespace Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.PriceList
{
    public class PriceListMiniModel 
    {
        [PropertyPresentation("String", "Pricelist Id", isEditable: false, isFilterable: true, isGridColumn: true, sortOrder: 2000)]
        public int ID { get; set; }

        [PropertyPresentation("String", "Pricelist identifier", isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 2001)]
        public string Identifier { get; set; }

        [PropertyPresentation("String", "Pricelist name", languageDependant: true, isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 2003)]
        public string Name { get; set; }
    }
}
