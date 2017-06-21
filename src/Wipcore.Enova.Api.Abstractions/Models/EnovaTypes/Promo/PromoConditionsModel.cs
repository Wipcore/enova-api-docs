namespace Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.Promo
{
    public class PromoConditionsModel
    {
        /*first row*/
        public bool BuyFor { get; set; }

        public decimal BuyForAmount { get; set; }

        public string BuyForAmountCurrency { get; set; }

        public bool BuyForAmountIncludeTax { get; set; }

        /*second row*/
        public bool BuyProduct { get; set; }

        public double BuyProductQuantity { get; set; }

        public string BuyProductIdentifier { get; set; }

        /*third row*/
        public bool BuyForFromCategory { get; set; }

        public decimal BuyForAmountFromCategory { get; set; }

        public string BuyForCurrencyFromCategory { get; set; }

        public bool BuyForIncludeTaxFromCategory { get; set; }

        public string BuyForFromCategoryIdentifier { get; set; }

        /*fourth row */

        public bool BuyFromCategory { get; set; }

        public double BuyFromCategoryQuantity { get; set; }

        public string BuyFromCategoryIdentifier { get; set; }
    }
}