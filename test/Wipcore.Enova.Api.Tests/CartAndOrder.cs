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
        [InlineData(new object[] { "", "", "unittestcartNoSave", "GF_GPU_880", null, null, null, null, false })]
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
            if(!String.IsNullOrEmpty(customerAlias))
                _testService.LoginCustomer(_client, customerAlias, customerPassword);

            //get prices of the products directly
            var price1 = GetProductPrice(product1Identifier);
            var price2 = product2Identifier == null ? 0 : GetProductPrice(product2Identifier);
            var totalPrice = price1 * product1Quantity + price2 * product2Quantity;

            //build and post cart 
            var cart = BuildCart(customerAlias, cartIdentifier, product1Identifier, product2Identifier, product1Quantity, product2Quantity, promoPassword, shippingType, paymentType);
            var postedCart = PutCart(cart, saveCart);

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
                Assert.NotNull(Get("carts", cartIdentifier));
            }
            else
            {
                Assert.Null(Get("carts",cartIdentifier));
            }

        }

        [Theory]
        [InlineData(new object[] { "69990002", "password", "unittestcartslim", "GF_GPU_880", null, 3, 1, null, null, null })]
        [InlineData(new object[] { "69990002", "password", "unittestcart", "GF_GPU_880", "KarinkjolenArtikel", 3, 1, "kjol", "OrdinaryShipment", "InvoicePaymentIdentifier"})]
        public void CanClearExistingCart(string customerAlias, string customerPassword, string cartIdentifier,string product1Identifier, string product2Identifier, int product1Quantity, int product2Quantity,
            string promoPassword, string shippingType, string paymentType)
        {
            //login
            _testService.LoginCustomer(_client, customerAlias, customerPassword);
            
            //make sure it exists
            var cart = BuildCart(customerAlias, cartIdentifier, product1Identifier, product2Identifier, product1Quantity, product2Quantity, promoPassword, shippingType, paymentType);
            PutCart(cart, true);

            //then remove all its items
            var clearCart = BuildCart(customerAlias, cartIdentifier, product1Identifier, product2Identifier, product1Quantity, product2Quantity, promoPassword, shippingType, paymentType, true);
            var clearedCart = PutCart(clearCart, true);

            Assert.Null(clearedCart["ShippingCartItem"]);
            Assert.Null(clearedCart["PaymentCartItem"]);
            Assert.Equal(0, JsonConvert.DeserializeAnonymousType(clearedCart["ProductCartItems"].ToString(), new[] { new { ProductIdentifier = "", PriceInclTax = 0m } }).Length);
            Assert.Equal(0, JsonConvert.DeserializeAnonymousType(clearedCart["PromoCartItems"].ToString(), new[] { new { ProductIdentifier = "", PriceInclTax = 0m } }).Length);

            var receivedTotalPrice = Convert.ToDecimal(clearedCart["TotalPriceInclTax"]);
            Assert.Equal(0, receivedTotalPrice);
        }

        [Theory]
        [InlineData(new object[] { "69990002", "password", "unittestorder", "GF_GPU_880", null, null, null, null })]
        [InlineData(new object[] { "69990002", "password", "unittestorder", "GF_GPU_880", "KarinkjolenArtikel", "kjol", "OrdinaryShipment", "InvoicePaymentIdentifier"})]
        public void CanCreateOrderFromCart(string customerAlias, string customerPassword, string orderIdentifier, string product1Identifier, string product2Identifier, string promoPassword,
            string shippingType, string paymentType)
        {
            var product1Quantity = _random.Next(1, 10);
            var product2Quantity = _random.Next(1, 10);

            var product1Comment = _random.Next(1, 1000).ToString();
            var product2Comment = _random.Next(1, 1000).ToString();

            //login
            _testService.LoginCustomer(_client, customerAlias, customerPassword);

            //get prices of the products directly
            var price1 = GetProductPrice(product1Identifier);
            var price2 = product2Identifier == null ? 0 : GetProductPrice(product2Identifier);
            var totalPrice = price1 * product1Quantity + price2 * product2Quantity;
            
            //build cart, create order
            var cart = BuildCart(customerAlias, "", product1Identifier, product2Identifier, product1Quantity, product2Quantity, promoPassword, shippingType, paymentType, false, product1Comment, product2Comment);
            var orderId = PostCartAsOrder(cart);

            var properties = "ShippingOrderItem,PaymentOrderItem,ProductOrderItems,PromoOrderItems,TotalPriceInclTax";
            var order = Get("orders", null, orderId, properties);
            Assert.NotNull(order);

            //verify order

            var recivedShipping = shippingType == null ? null : JsonConvert.DeserializeAnonymousType(order["ShippingOrderItem"].ToString(), new { ShippingIdentifier = "", PriceInclTax = 0m });
            var recivedShippingCost = shippingType == null ? 0 : recivedShipping.PriceInclTax;
            if (shippingType != null)
                Assert.Equal(shippingType, recivedShipping.ShippingIdentifier);

            var recivedPayment = paymentType == null ? null : JsonConvert.DeserializeAnonymousType(order["PaymentOrderItem"].ToString(), new { PaymentIdentifier = "", PriceInclTax = 0m });
            var recivedPaymentCost = paymentType == null ? 0 : recivedPayment.PriceInclTax;
            if (recivedPayment != null)
                Assert.Equal(paymentType, recivedPayment.PaymentIdentifier);

            var productRows = JsonConvert.DeserializeAnonymousType(order["ProductOrderItems"].ToString(), new[] { new { ProductIdentifier = "", PriceInclTax = 0m, Comment = "" } });
            var receivedPrice1 = productRows.First(x => x.ProductIdentifier == product1Identifier).PriceInclTax;
            var receivedPrice2 = product2Identifier == null ? 0 : productRows.First(x => x.ProductIdentifier == product2Identifier).PriceInclTax;

            var promoDiscount = promoPassword == null ? 0 : JsonConvert.DeserializeAnonymousType(order["PromoOrderItems"].ToString(), new[] { new { PriceInclTax = 0m } }).First().PriceInclTax;

            var receivedTotalPrice = Convert.ToDecimal(order["TotalPriceInclTax"]);
            
            Assert.Equal(receivedPrice1, price1);
            Assert.Equal(receivedPrice2, price2);
            Assert.Equal(totalPrice + promoDiscount + recivedShippingCost + recivedPaymentCost, receivedTotalPrice);

            Assert.Equal(product1Comment, productRows.First(x => x.ProductIdentifier == product1Identifier).Comment);
            if(product2Identifier != null)
                Assert.Equal(product2Comment, productRows.First(x => x.ProductIdentifier == product2Identifier).Comment);

            Delete("orders", orderId);
            Assert.Null(Get("orders", null, orderId));
        }

        [Fact]
        public void CanChangeOrderStatus()
        {
            const string newStatus = "NEW_INTERNET";
            const string changedStatus = "VERIFIED_INTERNET";
            
            var order = new Dictionary<string, object>() { {"ID", 0}, { "ShippingStatus", newStatus } };
            var json = JsonConvert.SerializeObject(order);
            var content = new StringContent(json, new UTF8Encoding(), "application/json");
            var url = $"orders";
            var response = _testService.AdminClient.PutAsync(url, content).Result.Content.ReadAsStringAsync().Result;
            order = JsonConvert.DeserializeObject<Dictionary<string, object>>(response);

            Assert.Equal(order["ShippingStatus"], newStatus);

            order["ShippingStatus"] = changedStatus;
            json = JsonConvert.SerializeObject(order);
            content = new StringContent(json, new UTF8Encoding(), "application/json");
            response = _testService.AdminClient.PutAsync(url, content).Result.Content.ReadAsStringAsync().Result;
            order = JsonConvert.DeserializeObject<Dictionary<string, object>>(response);

            Assert.Equal(order["ShippingStatus"], changedStatus);

            var orderId = Convert.ToInt32(order["ID"]);
            Delete("orders", orderId);
            Assert.Null(Get("orders", null, orderId));
        }

        private IDictionary<string, object> Get(string resource, string identifier, int id = 0, string properties = null)
        {
            try
            {
                var url = id > 0 ? $"{resource}/id-{id}" : $"{resource}/{identifier}";
                if(properties != null)
                    url += "?properties="+properties;

                var response = _client.GetAsync(url).Result.Content.ReadAsStringAsync().Result;
                var item = JsonConvert.DeserializeObject<IDictionary<string, object>>(response);
                return item;
            }
            catch
            {
                return null;
            }
        }

        private void Delete(string resource, int id)
        {
            try
            {
                var url = $"{resource}/id-{id}";
                var response = _testService.AdminClient.DeleteAsync(url).Result;
                response.EnsureSuccessStatusCode();
            }
            catch
            {
            }

        }

        private IDictionary<string, object> PutCart(Dictionary<string, object> cart, bool saveCart)
        {
            var json = JsonConvert.SerializeObject(cart);
            var content = new StringContent(json, new UTF8Encoding(), "application/json");

            var url = $"carts?calculateOnly="+!saveCart;
            var response = _client.PutAsync(url, content).Result.Content.ReadAsStringAsync().Result;
            var item = JsonConvert.DeserializeObject<IDictionary<string, object>>(response);
            return item;
        }

        private int PostCartAsOrder(Dictionary<string, object> cart)
        {
            var json = JsonConvert.SerializeObject(cart);
            var content = new StringContent(json, new UTF8Encoding(), "application/json");

            var url = $"carts/createorder";
            var response = _client.PutAsync(url, content).Result.Content.ReadAsStringAsync().Result;
            var orderId = JsonConvert.DeserializeObject<int>(response);
            return orderId;
        }

        private static Dictionary<string, object> BuildCart(string customerIdentifier, string cartIdentifier, string product1Identifier, string product2Identifier, int product1Quantity, 
            int product2Quantity, string promoPassword, string shippingType, string paymentType, bool deleteItems = false, string product1Comment = null, string product2Comment = null)
        {
            var products = new List<object>();
            var cart = new Dictionary<string, object>(){{"Identifier", cartIdentifier},{"CustomerIdentifier", customerIdentifier},{"ProductCartItems", products}, { "TotalPriceInclTax", 0 },
                { "PromoCartItems", null }, {"ShippingCartItem", null}, {"PaymentCartItem", null} };

            products.Add(new Dictionary<string, object>(){{"ProductIdentifier", product1Identifier},{"Quantity", product1Quantity}, {"MarkForDelete", deleteItems}, {"Comment", product1Comment ?? String.Empty} });
            if (product2Identifier != null)
                products.Add(new Dictionary<string, object>(){{"ProductIdentifier", product2Identifier},{"Quantity", product2Quantity}, { "MarkForDelete", deleteItems }, { "Comment", product2Comment ?? String.Empty } });

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
