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
    public class PromoResultMapper : IPropertyMapper
    {
        public bool PostSaveSet => false;
        public List<string> Names => new List<string>() { "Results" };
        public Type Type => typeof(EnovaPromo);
        public bool InheritMapper => true;

        public int Priority => 0;
        public MapType MapType => MapType.MapFromAndToEnovaAllowed;

        public object GetEnovaProperty(BaseObject obj, string propertyName)
        {
            var promo = (EnovaPromo) obj; 
            var orderDiscountResult = promo.GetResults(typeof(EnovaOrderDiscountPromoResult)).Cast<EnovaOrderDiscountPromoResult>().FirstOrDefault();
            var freeShippingResult = promo.GetResults(typeof(EnovaFreeShippingPromoResult)).Cast<EnovaFreeShippingPromoResult>().FirstOrDefault();
            var productQuantityResult = promo.GetResults(typeof(EnovaProductQuantityPromoResult)).Cast<EnovaProductQuantityPromoResult>().FirstOrDefault();
            var freeProductResult = promo.GetResults(typeof(EnovaFreeProductPromoResult)).Cast<EnovaFreeProductPromoResult>().FirstOrDefault();
            var productDiscountResult = promo.GetResults(typeof(EnovaDiscountedProductPromoResult)).Cast<EnovaDiscountedProductPromoResult>().FirstOrDefault();
            var fixedPriceResult = promo.GetResults(typeof(EnovaFixedPricePromoResult)).Cast<EnovaFixedPricePromoResult>().FirstOrDefault();
            var sectionDiscount = promo.GetResults(typeof(EnovaSectionDiscountPromoResult)).Cast<EnovaSectionDiscountPromoResult>().FirstOrDefault();

            Currency orderDiscountCurrency = null;
            int decimals;
            var orderDiscountAmount = orderDiscountResult == null ? 0m : orderDiscountResult.GetAmount(out decimals, ref orderDiscountCurrency);

            Currency productDiscountCurrency = null;
            var productDiscountPrice = productDiscountResult == null ? 0m : productDiscountResult.GetPrice(out decimals, ref productDiscountCurrency);

            Currency fixedPriceCurrency = null;
            var fixedPrice = fixedPriceResult == null ? 0m : fixedPriceResult.GetPrice(out decimals, ref fixedPriceCurrency);

            var resultModel = new
            {
                OrderDiscount = orderDiscountResult != null,
                OrderDiscountPercent = orderDiscountResult?.Percent ?? 0,
                OrderDiscountAmount = orderDiscountAmount,
                OrderDiscountCurrency = orderDiscountCurrency?.Identifier,
                OrderDiscountIncludeTax = orderDiscountResult?.TaxIncluded == true,

                FreeShipping = freeShippingResult != null,

                AdditionalFreeProduct = productQuantityResult != null,
                AdditionalFreeProductQuantity = productQuantityResult?.Quantity ?? 0d,

                FreeProduct = freeProductResult != null,
                FreeProductQuantity = freeProductResult?.Quantity ?? 0d,
                FreeProductIdentifier = freeProductResult?.Product?.Identifier,

                ProductDiscount = productDiscountResult != null,
                ProductDiscountQuantity = productDiscountResult?.MaxQuantity ?? 0d,
                ProductDiscountIdentifier = productDiscountResult?.Product?.Identifier,
                ProductDiscountAmount = productDiscountPrice,
                ProductDiscountCurrency = productDiscountCurrency?.Identifier,

                FixedPrice = fixedPriceResult != null,
                FixedPriceAmount = fixedPrice,
                FixedPriceCurrency = fixedPriceCurrency?.Identifier,
                FixedPriceIncludeTax = fixedPriceResult?.TaxIncluded == true,

                SectionDiscount = sectionDiscount != null,
                SectionDiscountPercent = sectionDiscount?.Percent ?? 0d,
                SectionDiscountQuantity = sectionDiscount?.MaxQuantity ?? 0d,
                SectionDiscountIdentifier = sectionDiscount?.Section?.Identifier
            };

            return resultModel;
        }

        public void SetEnovaProperty(BaseObject obj, string propertyName, object value, IDictionary<string, object> otherValues)
        {
            if (value == null)
                return;

            var context = obj.GetContext();
            var promo = (EnovaPromo)obj;
            var orderDiscountResult = promo.GetResults(typeof(EnovaOrderDiscountPromoResult)).Cast<EnovaOrderDiscountPromoResult>().FirstOrDefault();
            var freeShippingResult = promo.GetResults(typeof(EnovaFreeShippingPromoResult)).Cast<EnovaFreeShippingPromoResult>().FirstOrDefault();
            var productQuantityResult = promo.GetResults(typeof(EnovaProductQuantityPromoResult)).Cast<EnovaProductQuantityPromoResult>().FirstOrDefault();
            var freeProductResult = promo.GetResults(typeof(EnovaFreeProductPromoResult)).Cast<EnovaFreeProductPromoResult>().FirstOrDefault();
            var productDiscountResult = promo.GetResults(typeof(EnovaDiscountedProductPromoResult)).Cast<EnovaDiscountedProductPromoResult>().FirstOrDefault();
            var fixedPriceResult = promo.GetResults(typeof(EnovaFixedPricePromoResult)).Cast<EnovaFixedPricePromoResult>().FirstOrDefault();
            var sectionDiscount = promo.GetResults(typeof(EnovaSectionDiscountPromoResult)).Cast<EnovaSectionDiscountPromoResult>().FirstOrDefault();

            var resultsModel = JsonConvert.DeserializeAnonymousType(value.ToString(), new
            {
                OrderDiscount = false,
                OrderDiscountPercent = 0d,
                OrderDiscountAmount = 0m,
                OrderDiscountCurrency = String.Empty,
                OrderDiscountIncludeTax = false,

                FreeShipping = false,

                AdditionalFreeProduct = false,
                AdditionalFreeProductQuantity = 0d,

                FreeProduct = false,
                FreeProductQuantity = 0d,
                FreeProductIdentifier = String.Empty,

                ProductDiscount = false,
                ProductDiscountQuantity = 0d,
                ProductDiscountIdentifier = String.Empty,
                ProductDiscountAmount = 0m,
                ProductDiscountCurrency = String.Empty,

                FixedPrice = false,
                FixedPriceAmount = 0m,
                FixedPriceCurrency = String.Empty,
                FixedPriceIncludeTax = false,

                SectionDiscount = false,
                SectionDiscountPercent = 0d,
                SectionDiscountQuantity = 0d,
                SectionDiscountIdentifier = String.Empty
            });

            //ORDER DISCOUNT
            if (!resultsModel.OrderDiscount && orderDiscountResult != null)
                promo.DeleteResult(orderDiscountResult);
            else if (resultsModel.OrderDiscount)
            {
                if (orderDiscountResult == null)
                {
                    orderDiscountResult = EnovaObjectCreationHelper.CreateNew<EnovaOrderDiscountPromoResult>(context);
                    promo.AddResult(orderDiscountResult);
                }

                if (String.IsNullOrEmpty(resultsModel.OrderDiscountCurrency))
                    orderDiscountResult.SetAmount(resultsModel.OrderDiscountAmount);
                else
                    orderDiscountResult.SetAmount(resultsModel.OrderDiscountAmount, EnovaCurrency.Find(context, resultsModel.OrderDiscountCurrency));
                orderDiscountResult.TaxIncluded = resultsModel.OrderDiscountIncludeTax;
                orderDiscountResult.Percent = resultsModel.OrderDiscountPercent;
            }

            //FREE SHIPPING
            if (!resultsModel.FreeShipping && freeShippingResult != null)
                promo.DeleteResult(freeShippingResult);
            else if (resultsModel.FreeShipping)
            {
                if (freeShippingResult == null)
                {
                    freeShippingResult = EnovaObjectCreationHelper.CreateNew<EnovaFreeShippingPromoResult>(context);
                    promo.AddResult(freeShippingResult);
                }
            }

            //PRODUCT QUANTITY
            if (!resultsModel.AdditionalFreeProduct && productQuantityResult != null)
                promo.DeleteResult(productQuantityResult);
            else if (resultsModel.AdditionalFreeProduct)
            {
                if (productQuantityResult == null)
                {
                    productQuantityResult = EnovaObjectCreationHelper.CreateNew<EnovaProductQuantityPromoResult>(context);
                    promo.AddResult(productQuantityResult);
                }
                productQuantityResult.Quantity = resultsModel.AdditionalFreeProductQuantity;
            }

            //FREE PRODUCT
            if (!resultsModel.FreeProduct && freeProductResult != null)
                promo.DeleteResult(freeProductResult);
            else if (resultsModel.FreeProduct)
            {
                if (freeProductResult == null)
                {
                    freeProductResult = EnovaObjectCreationHelper.CreateNew<EnovaFreeProductPromoResult>(context);
                    promo.AddResult(freeProductResult);
                }
                freeProductResult.Quantity = resultsModel.FreeProductQuantity;
                freeProductResult.Product = context.FindObject<EnovaBaseProduct>(resultsModel.FreeProductIdentifier);
            }

            //PRODUCT DISCOUNT
            if (!resultsModel.ProductDiscount && productDiscountResult != null)
                promo.DeleteResult(productDiscountResult);
            else if (resultsModel.ProductDiscount)
            {
                if (productDiscountResult == null)
                {
                    productDiscountResult = EnovaObjectCreationHelper.CreateNew<EnovaDiscountedProductPromoResult>(context);
                    promo.AddResult(productDiscountResult);
                }
                productDiscountResult.MaxQuantity = resultsModel.ProductDiscountQuantity;
                productDiscountResult.Product = context.FindObject<EnovaBaseProduct>(resultsModel.ProductDiscountIdentifier);
                if(String.IsNullOrEmpty(resultsModel.ProductDiscountCurrency))
                    productDiscountResult.SetPrice(resultsModel.ProductDiscountAmount);
                else
                    productDiscountResult.SetPrice(resultsModel.ProductDiscountAmount, EnovaCurrency.Find(context, resultsModel.ProductDiscountCurrency));
            }

            //FIXED PRICE
            if (!resultsModel.FixedPrice && fixedPriceResult != null)
                promo.DeleteResult(fixedPriceResult);
            else if (resultsModel.FixedPrice)
            {
                if (fixedPriceResult == null)
                {
                    fixedPriceResult = EnovaObjectCreationHelper.CreateNew<EnovaFixedPricePromoResult>(context);
                    promo.AddResult(fixedPriceResult);
                }
                fixedPriceResult.TaxIncluded = resultsModel.FixedPriceIncludeTax;
                if (String.IsNullOrEmpty(resultsModel.FixedPriceCurrency))
                    fixedPriceResult.SetPrice(resultsModel.FixedPriceAmount);
                else
                    fixedPriceResult.SetPrice(resultsModel.FixedPriceAmount, EnovaCurrency.Find(context, resultsModel.FixedPriceCurrency));
            }

            //SECTION DISCOUNT
            if (!resultsModel.SectionDiscount && sectionDiscount != null)
                promo.DeleteResult(sectionDiscount);
            else if (resultsModel.SectionDiscount)
            {
                if (sectionDiscount == null)
                {
                    sectionDiscount = EnovaObjectCreationHelper.CreateNew<EnovaSectionDiscountPromoResult>(context);
                    promo.AddResult(sectionDiscount);
                }
                sectionDiscount.Percent = resultsModel.SectionDiscountPercent;
                sectionDiscount.MaxQuantity = resultsModel.SectionDiscountQuantity;
                sectionDiscount.Section = context.FindObject<EnovaBaseProductSection>(resultsModel.SectionDiscountIdentifier);
            }

        }
    }
}
