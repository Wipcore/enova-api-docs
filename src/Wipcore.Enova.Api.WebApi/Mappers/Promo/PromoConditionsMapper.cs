using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Interfaces;
using Wipcore.Enova.Api.WebApi.Helpers;
using Wipcore.Enova.Core;
using Wipcore.Enova.Generics;

namespace Wipcore.eNova.Api.WebApi.Mappers.Promo
{
    public class PromoConditionsMapper : IPropertyMapper
    {
        public bool PostSaveSet => false;
        public List<string> Names => new List<string>() { "Conditions" };
        public Type Type => typeof(EnovaPromo);
        public bool InheritMapper => true;

        public int Priority => 0;
        public MapType MapType => MapType.MapFromAndToEnovaAllowed;

        public object GetEnovaProperty(BaseObject obj, string propertyName)
        {
            var promo = (EnovaPromo) obj;
            var amountCondition = promo.GetConditions(typeof (EnovaOrderAmountPromoCondition)).Cast<EnovaOrderAmountPromoCondition>().FirstOrDefault();
            var productCondition = promo.GetConditions(typeof(EnovaProductQuantityPromoCondition)).Cast<EnovaProductQuantityPromoCondition>().FirstOrDefault();
            var amountCategoryCondition = promo.GetConditions(typeof(EnovaSectionAmountPromoCondition)).Cast<EnovaSectionAmountPromoCondition>().FirstOrDefault();
            var categoryCondition = promo.GetConditions(typeof(EnovaSectionQuantityPromoCondition)).Cast<EnovaSectionQuantityPromoCondition>().FirstOrDefault();

            Currency amountCurrency = null;
            int decimals;
            var amount = amountCondition == null ? 0m : amountCondition.GetAmount(out decimals, ref amountCurrency);

            Currency categoryAountCurrency = null;
            var categoryAmount = amountCategoryCondition == null ? 0m : amountCategoryCondition.GetAmount(out decimals, ref categoryAountCurrency);

            var conditionsModel = new
            {
                BuyFor = amountCondition != null,
                BuyForAmount = amount,
                BuyForAmountCurrency = amountCurrency?.Identifier,
                BuyForAmountIncludeTax = amountCondition?.TaxIncluded == true,

                BuyProduct = productCondition != null,
                BuyProductQuantity = productCondition?.Quantity ?? 0,
                BuyProductIdentifier = productCondition?.Product?.Identifier,

                BuyForFromCategory = amountCategoryCondition != null,
                BuyForAmountFromCategory = categoryAmount,
                BuyForCurrencyFromCategory = categoryAountCurrency?.Identifier,
                BuyForIncludeTaxFromCategory = amountCategoryCondition?.TaxIncluded == true,
                BuyForFromCategoryIdentifier = amountCategoryCondition?.Section?.Identifier,

                BuyFromCategory = categoryCondition != null,
                BuyFromCategoryQuantity = categoryCondition?.Quantity ?? 0,
                BuyFromCategoryIdentifier = categoryCondition?.Section?.Identifier,
            };

            return conditionsModel;
        }

        public void SetEnovaProperty(BaseObject obj, string propertyName, object value, IDictionary<string, object> otherValues)
        {
            var context = obj.GetContext();
            var promo = (EnovaPromo)obj;
            var amountCondition = promo.GetConditions(typeof(EnovaOrderAmountPromoCondition)).Cast<EnovaOrderAmountPromoCondition>().FirstOrDefault();
            var productCondition = promo.GetConditions(typeof(EnovaProductQuantityPromoCondition)).Cast<EnovaProductQuantityPromoCondition>().FirstOrDefault();
            var amountCategoryCondition = promo.GetConditions(typeof(EnovaSectionAmountPromoCondition)).Cast<EnovaSectionAmountPromoCondition>().FirstOrDefault();
            var categoryCondition = promo.GetConditions(typeof(EnovaSectionQuantityPromoCondition)).Cast<EnovaSectionQuantityPromoCondition>().FirstOrDefault();

            var conditionsModel = JsonConvert.DeserializeAnonymousType(value.ToString(), new {
                BuyFor = false,
                BuyForAmount = 0m,
                BuyForAmountCurrency = String.Empty,
                BuyForAmountIncludeTax = false,

                BuyProduct = false,
                BuyProductQuantity = 0d,
                BuyProductIdentifier = String.Empty,

                BuyForFromCategory = false,
                BuyForAmountFromCategory = 0m,
                BuyForCurrencyFromCategory = String.Empty,
                BuyForIncludeTaxFromCategory = false,
                BuyForFromCategoryIdentifier = String.Empty,

                BuyFromCategory = false,
                BuyFromCategoryQuantity = 0d,
                BuyFromCategoryIdentifier = String.Empty
            });

            //BUY FOR
            if(!conditionsModel.BuyFor && amountCondition != null)
                promo.DeleteCondition(amountCondition);
            else if(conditionsModel.BuyFor)
            {
                if (amountCondition == null)
                {
                    amountCondition = EnovaObjectCreationHelper.CreateNew<EnovaOrderAmountPromoCondition>(context);
                    promo.AddCondition(amountCondition);
                }
                
                if(String.IsNullOrEmpty(conditionsModel.BuyForAmountCurrency))
                    amountCondition.SetAmount(conditionsModel.BuyForAmount);
                else
                    amountCondition.SetAmount(conditionsModel.BuyForAmount, EnovaCurrency.Find(context, conditionsModel.BuyForAmountCurrency));
                amountCondition.TaxIncluded = conditionsModel.BuyForAmountIncludeTax;
            }

            //BUY PRODUCT
            if (!conditionsModel.BuyProduct && productCondition != null)
                promo.DeleteCondition(productCondition);
            else if (conditionsModel.BuyProduct)
            {
                if (productCondition == null)
                {
                    productCondition = EnovaObjectCreationHelper.CreateNew<EnovaProductQuantityPromoCondition>(context);
                    promo.AddCondition(productCondition);
                }

                productCondition.Quantity = conditionsModel.BuyProductQuantity;
                productCondition.Product = context.FindObject<EnovaBaseProduct>(conditionsModel.BuyProductIdentifier);
            }

            //BUY AMOUNT FROM CATEGORY
            if (!conditionsModel.BuyForFromCategory && amountCategoryCondition != null)
                promo.DeleteCondition(amountCategoryCondition);
            else if (conditionsModel.BuyForFromCategory)
            {
                if (amountCategoryCondition == null)
                {
                    amountCategoryCondition = EnovaObjectCreationHelper.CreateNew<EnovaSectionAmountPromoCondition>(context);
                    promo.AddCondition(amountCategoryCondition);
                }

                amountCategoryCondition.TaxIncluded = conditionsModel.BuyForIncludeTaxFromCategory;

                if (String.IsNullOrEmpty(conditionsModel.BuyForCurrencyFromCategory))
                    amountCategoryCondition.SetAmount(conditionsModel.BuyForAmountFromCategory);
                else
                    amountCategoryCondition.SetAmount(conditionsModel.BuyForAmountFromCategory, EnovaCurrency.Find(context, conditionsModel.BuyForCurrencyFromCategory));
                
                amountCategoryCondition.Section = context.FindObject<EnovaBaseSection>(conditionsModel.BuyForFromCategoryIdentifier);
            }

            //BUY FROM CATEGORY
            if (!conditionsModel.BuyFromCategory && categoryCondition != null)
                promo.DeleteCondition(categoryCondition);
            else if (conditionsModel.BuyFromCategory)
            {
                if (categoryCondition == null)
                {
                    categoryCondition = EnovaObjectCreationHelper.CreateNew<EnovaSectionQuantityPromoCondition>(context);
                    promo.AddCondition(categoryCondition);
                }

                categoryCondition.Section = context.FindObject<EnovaBaseSection>(conditionsModel.BuyFromCategoryIdentifier);
                categoryCondition.Quantity = conditionsModel.BuyFromCategoryQuantity;
            }
        }
    }
}
