namespace Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.Promo
{
    public class PromoResultModel
    {
        //first row
        public bool OrderDiscount { get; set; }
        public double OrderDiscountPercent { get; set; }
        public decimal OrderDiscountAmount { get; set; }
        public string OrderDiscountCurrency { get; set; }
        public bool OrderDiscountIncludeTax { get; set; }

        //second row
        public bool FreeShipping { get; set; }

        //third row
        public bool AdditionalFreeProduct { get; set; }
        public double AdditionalFreeProductQuantity { get; set; }

        //fourth row
        public bool FreeProduct { get; set; }
        public double FreeProductQuantity { get; set; }
        public string FreeProductIdentifier { get; set; }

        //fifth row
        public bool ProductDiscount { get; set; }
        public double ProductDiscountQuantity { get; set; }
        public string ProductDiscountIdentifier { get; set; }
        public decimal ProductDiscountAmount { get; set; }
        public string ProductDiscountCurrency { get; set; }

        //sixth row
        public bool FixedPrice { get; set; }
        public decimal FixedPriceAmount { get; set; }
        public string FixedPriceCurrency { get; set; }
        public bool FixedPriceIncludeTax { get; set; }

        //seventh row
        public bool SectionDiscount { get; set; }
        public double SectionDiscountPercent { get; set; }
        public double SectionDiscountQuantity { get; set; }
        public string SectionDiscountIdentifier { get; set; }


    }
}
