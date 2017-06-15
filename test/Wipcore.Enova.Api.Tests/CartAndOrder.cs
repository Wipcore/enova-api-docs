using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xunit;

namespace Wipcore.Enova.Api.Tests
{
    [Collection("WebApiCollection")]
    public class CartAndOrder : IClassFixture<TestService>
    {
        private readonly TestService _testService;
        private readonly HttpClient _client;
        private readonly Random _random = new Random();

        public CartAndOrder(TestService testService)
        {
            _testService = testService;
            _client = testService.GetNewClient();
        }


        [Theory]
        [InlineData(new object[] { "69990002", "password", "unittestcartNoSave", "GF_GPU_880", null, null, null, null, false })]
        [InlineData(new object[] { "69990002", "password", "unittestcartNoSave", "GF_GPU_880", "KarinkjolenArtikel", null, null, null, false })]
        [InlineData(new object[] { "69990002", "password", "unittestcartNoSave", "GF_GPU_880", "KarinkjolenArtikel", "kjol", null, null, false })]
        [InlineData(new object[] { "69990002", "password", "unittestcartNoSave", "GF_GPU_880", "KarinkjolenArtikel", "kjol", "OrdinaryShipment", null, false })]
        [InlineData(new object[] { "69990002", "password", "unittestcartNoSave", "GF_GPU_880", "KarinkjolenArtikel", "kjol", "OrdinaryShipment", "InvoicePaymentIdentifier", false })]
        [InlineData(new object[] { "69990002", "password", "unittestcart", "GF_GPU_880", "KarinkjolenArtikel", "kjol", "OrdinaryShipment", "InvoicePaymentIdentifier", true })]

        public void CanCalculateCart(string customerAlias, string customerPassword, string cartIdentifier, string product1Identifier, string product2Identifier, string promoPassword, 
            string shippingType, string paymentType, bool saveCart)
        {
            var product1Quantity = _random.Next(1, 10);
            var product2Quantity = _random.Next(1, 10);

            //login
            _testService.LoginCustomer(_client, customerAlias, customerPassword);

            //get prices of the products directly
            var price1 = GetProductPrice(product1Identifier);
            var price2 = product2Identifier == null ? 0 : GetProductPrice(product2Identifier);

            var totalPrice = price1 * product1Quantity + price2 * product2Quantity;

            //build and post cart 
            var cart = BuildCart(customerAlias, cartIdentifier, product1Identifier, product2Identifier, product1Quantity, product2Quantity, promoPassword, shippingType, paymentType);
            var postedCart = PostCart(cart, saveCart);

            //verify cart
            
            var recivedShipping = shippingType == null ? null : JsonConvert.DeserializeAnonymousType(postedCart["ShippingCartItem"].ToString(),  new { ShippingIdentifier = "", PriceInclTax = 0m } );
            var recivedShippingCost = shippingType == null ? 0 : recivedShipping.PriceInclTax;
            if(shippingType != null)
                Assert.Equal(shippingType, recivedShipping.ShippingIdentifier);

            var recivedPayment = paymentType == null ? null : JsonConvert.DeserializeAnonymousType(postedCart["PaymentCartItem"].ToString(), new { PaymentIdentifier = "", PriceInclTax = 0m });
            var recivedPaymentCost = paymentType == null ? 0 : recivedPayment.PriceInclTax;
            if (recivedPayment != null)
                Assert.Equal(paymentType, recivedPayment.PaymentIdentifier);

            var productRows = JsonConvert.DeserializeAnonymousType(postedCart["ProductCartItems"].ToString(), new[] { new { ProductIdentifier = "", PriceInclTax = 0m } });
            var receivedPrice1 = productRows.First(x => x.ProductIdentifier == product1Identifier).PriceInclTax;
            var receivedPrice2 = product2Identifier == null ? 0 : productRows.First(x => x.ProductIdentifier == product2Identifier).PriceInclTax;

            var promoDiscount = promoPassword == null ? 0 : JsonConvert.DeserializeAnonymousType(postedCart["PromoCartItems"].ToString(), new[] { new { PriceInclTax = 0m } }).First().PriceInclTax;

            var receivedTotalPrice =  Convert.ToDecimal(postedCart["TotalPriceInclTax"]);

            Assert.Equal(receivedPrice1, price1);
            Assert.Equal(receivedPrice2, price2);
            Assert.Equal(totalPrice + promoDiscount + recivedShippingCost + recivedPaymentCost, receivedTotalPrice);

            
            if (saveCart)
            {
                Assert.NotNull(GetCart(cartIdentifier));
            }
            else
            {
                Assert.Null(GetCart(cartIdentifier));
            }

        }

        [Theory]
        [InlineData(new object[] { "69990002", "password", "unittestcart", "GF_GPU_880", "KarinkjolenArtikel", 3, 1, "kjol", "OrdinaryShipment", "InvoicePaymentIdentifier"})]
        public void CanClearExistingCart(string customerAlias, string customerPassword, string cartIdentifier,string product1Identifier, string product2Identifier, int product1Quantity, int product2Quantity,
            string promoPassword, string shippingType, string paymentType)
        {
            //login
            _testService.LoginCustomer(_client, customerAlias, customerPassword);

            //make sure it exists
            var cart = BuildCart(customerAlias, cartIdentifier, product1Identifier, product2Identifier, product1Quantity, product2Quantity, promoPassword, shippingType, paymentType);
            PostCart(cart, true);

            //then remove all its items
            var clearCart = BuildCart(customerAlias, cartIdentifier, product1Identifier, product2Identifier, product1Quantity, product2Quantity, promoPassword, shippingType, paymentType, true);
            var clearedCart = PostCart(clearCart, true);

            Assert.Null(clearedCart["ShippingCartItem"]);
            Assert.Null(clearedCart["PaymentCartItem"]);
            Assert.Equal(0, JsonConvert.DeserializeAnonymousType(clearedCart["ProductCartItems"].ToString(), new[] { new { ProductIdentifier = "", PriceInclTax = 0m } }).Length);
            Assert.Equal(0, JsonConvert.DeserializeAnonymousType(clearedCart["PromoCartItems"].ToString(), new[] { new { ProductIdentifier = "", PriceInclTax = 0m } }).Length);

            var receivedTotalPrice = Convert.ToDecimal(clearedCart["TotalPriceInclTax"]);
            Assert.Equal(0, receivedTotalPrice);
        }

        private IDictionary<string, object> GetCart(string identifier)
        {
            try
            {
                var url = $"carts/{identifier}";
                var response = _client.GetAsync(url).Result.Content.ReadAsStringAsync().Result;
                var item = JsonConvert.DeserializeObject<IDictionary<string, object>>(response);
                return item;
            }
            catch
            {
                return null;
            }
            
        }

        private IDictionary<string, object> PostCart(Dictionary<string, object> cart, bool saveCart)
        {
            var json = JsonConvert.SerializeObject(cart);
            var content = new StringContent(json, new UTF8Encoding(), "application/json");

            var url = $"carts?calculateOnly="+!saveCart;
            var response = _client.PutAsync(url, content).Result.Content.ReadAsStringAsync().Result;
            var item = JsonConvert.DeserializeObject<IDictionary<string, object>>(response);
            return item;
        }

        private static Dictionary<string, object> BuildCart(string customerIdentifier, string cartIdentifier, string product1Identifier, string product2Identifier, int product1Quantity, 
            int product2Quantity, string promoPassword, string shippingType, string paymentType, bool deleteItems = false)
        {
            var products = new List<object>();
            var cart = new Dictionary<string, object>(){{"Identifier", cartIdentifier},{"CustomerIdentifier", customerIdentifier},{"ProductCartItems", products}, { "TotalPriceInclTax", 0 },
                { "PromoCartItems", null }, {"ShippingCartItem", null}, {"PaymentCartItem", null} };

            products.Add(new Dictionary<string, object>(){{"ProductIdentifier", product1Identifier},{"Quantity", product1Quantity}, {"MarkForDelete", deleteItems} });
            if (product2Identifier != null)
                products.Add(new Dictionary<string, object>(){{"ProductIdentifier", product2Identifier},{"Quantity", product2Quantity}, { "MarkForDelete", deleteItems } });

            if (shippingType != null)
            {
                if(deleteItems)
                    cart["ShippingCartItem"] = new Dictionary<string, object>() { { "MarkForDelete", true } };
                else
                    cart.Add("NewShippingType", shippingType);
            }

            if (paymentType != null)
            {
                if(deleteItems)
                    cart["PaymentCartItem"] = new Dictionary<string, object>() { { "MarkForDelete", true } };
                else
                    cart.Add("NewPaymentType", paymentType);
            }
                
            if (promoPassword != null)
                cart.Add("PromoCode", promoPassword);
            return cart;
        }

        private decimal GetProductPrice(string identifier)
        {
            var url = $"products/{identifier}?properties=PriceInclTax";
            var response = _client.GetAsync(url).Result.Content.ReadAsStringAsync().Result;
            var price = JsonConvert.DeserializeObject<IDictionary<string, object>>(response).First().Value;

            return Convert.ToDecimal(price);
        }

    }
}
