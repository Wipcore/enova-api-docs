using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Wipcore.Enova.Api.Abstractions.Interfaces;
using Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.Cart;
using Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.Customer;
using Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.Order;
using Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.Product;
using Wipcore.Enova.Api.NetClient;
using Wipcore.Enova.Api.NETClient;

namespace Wipcore.Enova.Api.NetClient
{
    public class NetClientModule : Autofac.Module, IEnovaApiModule
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ApiClientAsync>().AsImplementedInterfaces();
            builder.RegisterType<ApiClient>().AsImplementedInterfaces();
            builder.RegisterType<ApiRepositoryAsync>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<ApiRepository>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<CartRepository<CartModel, OrderModel>>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<CartRepositoryAsync<CartModel, OrderModel>>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<CustomerRepository<CustomerModel, CartModel, OrderModel>>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<CustomerRepositoryAsync<CustomerModel, CartModel, OrderModel>>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<ProductRepository<ProductModel>>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<ProductRepositoryAsync<ProductModel>>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<OrderRepository<OrderModel>>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<OrderRepositoryAsync<OrderModel>>().AsImplementedInterfaces().SingleInstance();
        }

        public int Priority => 0;
    }
}
