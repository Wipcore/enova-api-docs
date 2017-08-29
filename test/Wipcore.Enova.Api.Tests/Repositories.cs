using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Wipcore.Enova.Api.NetClient;
using Wipcore.Enova.Api.Abstractions;
using Wipcore.Enova.Api.Abstractions.Interfaces;
using Wipcore.Enova.Api.Abstractions.Models;
using Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.Attribute;
using Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.Cart;
using Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.Order;
using Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.Product;
using Wipcore.Enova.Api.NetClient;
using Xunit;
using CartModel = Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.Cart.CartModel;
using CustomerModel = Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.Customer.CustomerModel;

namespace Wipcore.Enova.Api.Tests
{
    [Collection("WebApiCollection")]
    public class Repositories : IClassFixture<TestService>
    {
        private readonly TestService _testService;
        private readonly CustomerRepository<CustomerModel, CartModel, OrderModel> _customerRepository;
        private readonly Random _random = new Random();
        private readonly CartRepository<CartModel, OrderModel> _cartRepository;
        private readonly OrderRepository<OrderModel> _orderRepository;
        private readonly ProductRepository<ProductModel> _productRepository;

        public Repositories(TestService testService)
        {
            _testService = testService;
            _customerRepository = (CustomerRepository<CustomerModel, CartModel, OrderModel>)_testService.Server.Host.Services.GetService(typeof(CustomerRepository<CustomerModel, CartModel, OrderModel>));
            _cartRepository = (CartRepository<CartModel, OrderModel>)_testService.Server.Host.Services.GetService(typeof(CartRepository<CartModel, OrderModel>));
            _orderRepository = (OrderRepository<OrderModel>)_testService.Server.Host.Services.GetService(typeof(OrderRepository<OrderModel>));
            _productRepository = (ProductRepository<ProductModel>)_testService.Server.Host.Services.GetService(typeof(ProductRepository<ProductModel>));
        }

        private void SetupHttpContext(HttpContext context)
        {
            var accessor = (IHttpContextAccessor)_testService.Server.Host.Services.GetService(typeof(IHttpContextAccessor));
            accessor.HttpContext = context;
        }

        [Theory]
        [InlineData(new object[] { "69990002", "password" })]
        public void CanUpdateCustomerByRepo(string customerIdentifier, string password)
        {
            SetupHttpContext(new DefaultHttpContext() { TraceIdentifier = WipConstants.ElasticDeltaIndexHttpContextIdentifier });

            _customerRepository.LoginCustomer(customerIdentifier, password);

            var newSortOrder = _random.Next(1000);
            var model = new CustomerModel() { Identifier = customerIdentifier, SortOrder = newSortOrder, Alias = customerIdentifier };

            _customerRepository.UpdateCustomer(model);
            var savedModel = _customerRepository.GetSavedCustomer(customerIdentifier);
            Assert.Equal(newSortOrder, savedModel.SortOrder);
        }

        [Theory]
        [InlineData(new object[] { "69990002", "password" })]
        public void CanGetCustomerCartsByRepo(string customerIdentifier, string password)
        {
            SetupHttpContext(new DefaultHttpContext() { TraceIdentifier = WipConstants.ElasticDeltaIndexHttpContextIdentifier });

            _customerRepository.LoginCustomer(customerIdentifier, password);
            var customer = _customerRepository.GetSavedCustomer(customerIdentifier);
            
            var cartsByIdentifier = _customerRepository.GetCarts(customer.Identifier);
            var cartsById = _customerRepository.GetCarts(customer.ID);

            Assert.NotEqual(cartsById.Count, 0);
            cartsById.ForEach(x => Assert.NotEqual(x.ID, 0));

            Assert.NotEqual(cartsByIdentifier.Count, 0);
            cartsByIdentifier.ForEach(x => Assert.NotEqual(x.ID, 0));

            Assert.Equal(cartsById.Count, cartsByIdentifier.Count);
        }

        [Theory]
        [InlineData(new object[] { "69990002", "password" })]
        public void CanGetCustomerOrdersByRepo(string customerIdentifier, string password)
        {
            SetupHttpContext(new DefaultHttpContext() { TraceIdentifier = WipConstants.ElasticDeltaIndexHttpContextIdentifier });

            _customerRepository.LoginCustomer(customerIdentifier, password);
            var customer = _customerRepository.GetSavedCustomer(customerIdentifier);
            
            var ordersById = _customerRepository.GetOrders(customer.ID);
            var ordersByIdentifier = _customerRepository.GetOrders(customer.Identifier);
            var ordersWithStatus = _customerRepository.GetOrders(customer.Identifier, shippingStatusIdentifier: "NEW_INTERNET");

            Assert.NotEqual(ordersById.Count, 0);
            ordersById.ForEach(x => Assert.NotEqual(x.ID, 0));

            Assert.NotEqual(ordersByIdentifier.Count, 0);
            ordersByIdentifier.ForEach(x => Assert.NotEqual(x.ID, 0));

            Assert.Equal(ordersById.Count, ordersByIdentifier.Count);

            Assert.NotEqual(ordersWithStatus.Count, 0);
            Assert.True(ordersWithStatus.Count < ordersByIdentifier.Count);
        }

        [Theory]
        [InlineData(new object[] { "69990002", "GF_GPU_880", "KarinkjolenArtikel" })]
        public void CanUpdateAndCreateACartByRepo(string customerIdentifier, string productIdentifier1, string productIdentifier2)
        {
            var cartIdentifier = "unittestcartrep";
            SetupHttpContext(new DefaultHttpContext() { TraceIdentifier = WipConstants.ElasticIndexHttpContextIdentifier });

            var client = (IApiClient)_testService.Server.Host.Services.GetService(typeof(IApiClient));
            client.LoginAdmin("wadmin", "wadmin");

            //calculate
            var cart = new CartModel() {Identifier = cartIdentifier, CustomerIdentifier = customerIdentifier, ProductCartItems = new List<ProductCartItemModel>()
            { new ProductCartItemModel() {ProductIdentifier = productIdentifier1, Quantity = 3}, new ProductCartItemModel() {ProductIdentifier = productIdentifier2, Quantity = 2} } };

            var calculatedCart = _cartRepository.Calculate(cart);
            Assert.NotEqual(0, calculatedCart.TotalPriceExclTax);
            Assert.NotEqual(0, calculatedCart.ProductCartItems.Count);
            calculatedCart.ProductCartItems.ForEach(x => Assert.NotEqual(0, x.PriceExclTax));

            //save
            _cartRepository.CreateCart(cart);

            //get
            Assert.NotNull(_cartRepository.GetSavedCart(cartIdentifier));

            //turn into order
            var order = _cartRepository.TurnCartIntoOrder(cart);
            Assert.NotEqual(0, order.TotalPriceExclTax);
            
            //delete
            _cartRepository.DeleteCart(cartIdentifier);
            Assert.Null(_cartRepository.GetSavedCart(cartIdentifier));

            _orderRepository.DeleteOrder(order.ID);
            Assert.Null(_orderRepository.GetSavedOrder(order.ID));
        }

        [Theory]
        [InlineData(new object[] {"unittestrepoprod", "GF_GPU_880", "KarinkjolenArtikel" })]
        public void CanCrudProductsByRepo(string productIdentifier1, string productIdentifier2, string productIdentifier3)
        {
            SetupHttpContext(new DefaultHttpContext() { TraceIdentifier = WipConstants.ElasticIndexHttpContextIdentifier });

            var client = (IApiClient)_testService.Server.Host.Services.GetService(typeof(IApiClient));
            client.LoginAdmin("wadmin", "wadmin");

            //create
            var unittestprod = new ProductModel() { Identifier = productIdentifier1, Attributes = new List<AttributeValueModel>()
                { new AttributeValueModel() { Value = "UnGood", AttributeType = new AttributeTypeSimpleModel() {Identifier = "Attribute_smak" } } }};
            _productRepository.CreateProduct(unittestprod);

            //get
            var products = _productRepository.GetProducts(new List<string>() { productIdentifier1, productIdentifier2, productIdentifier3 });
            Assert.Equal(3, products.Count);
            Assert.Contains(productIdentifier1, products.Select(x => x.Identifier));
            Assert.Contains(productIdentifier2, products.Select(x => x.Identifier));
            Assert.Contains(productIdentifier3, products.Select(x => x.Identifier));

            //delete
            Assert.True(_productRepository.DeleteProduct(productIdentifier1));
            Assert.Null(_productRepository.GetProduct(productIdentifier1));
        }

        [Fact]
        public void CanPaginateByRepo()
        {
            SetupHttpContext(new DefaultHttpContext() { TraceIdentifier = WipConstants.ElasticIndexHttpContextIdentifier });

            var respondsHeaders = new ApiResponseHeadersModel();
            var query = new QueryModel() {Size = 5, Properties = "ID"};
            var products = _productRepository.GetProducts(query, null, respondsHeaders);
            var page1Ids = products.Select(x => x.ID).OrderBy(x => x).ToList();

            //next page
            products = _productRepository.GetNextProductPage(respondsHeaders);
            var page2Ids = products.Select(x => x.ID).OrderBy(x => x).ToList();

            //previous page
            products = _productRepository.GetPreviousProductPage(respondsHeaders);
            var page1IdsAagain = products.Select(x => x.ID).OrderBy(x => x).ToList();

            page1Ids.ForEach(x => Assert.DoesNotContain(x, page2Ids));

            page1Ids.ForEach(x => Assert.Contains(x, page1IdsAagain));

        }

    }
}
