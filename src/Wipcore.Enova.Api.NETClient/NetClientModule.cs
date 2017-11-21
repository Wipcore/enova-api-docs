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
            builder.RegisterType<ApiClientAsync>().AsImplementedInterfaces().AsSelf();
            builder.RegisterType<ApiClient>().AsImplementedInterfaces().AsSelf();
            builder.RegisterType<ApiRepositoryAsync>().AsImplementedInterfaces().AsSelf().SingleInstance();
            builder.RegisterType<ApiRepository>().AsImplementedInterfaces().AsSelf().SingleInstance();
            builder.RegisterType<CartRepository<CartModel, OrderModel>>().AsImplementedInterfaces().AsSelf().SingleInstance();
            builder.RegisterType<CartRepositoryAsync<CartModel, OrderModel>>().AsImplementedInterfaces().AsSelf().SingleInstance();
            builder.RegisterType<CustomerRepository<CustomerModel, CartModel, OrderModel>>().AsImplementedInterfaces().AsSelf().SingleInstance();
            builder.RegisterType<CustomerRepositoryAsync<CustomerModel, CartModel, OrderModel>>().AsImplementedInterfaces().AsSelf().SingleInstance();
            builder.RegisterType<ProductRepository<ProductModel>>().AsImplementedInterfaces().AsSelf().SingleInstance();
            builder.RegisterType<ProductRepositoryAsync<ProductModel>>().AsImplementedInterfaces().AsSelf().SingleInstance();
            builder.RegisterType<OrderRepository<OrderModel>>().AsImplementedInterfaces().AsSelf().SingleInstance();
            builder.RegisterType<OrderRepositoryAsync<OrderModel>>().AsImplementedInterfaces().AsSelf().SingleInstance();
        }

        public int Priority => 0;
    }
}
