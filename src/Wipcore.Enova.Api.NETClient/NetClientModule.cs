using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Wipcore.Enova.Api.Abstractions.Interfaces;
using Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.Cart;
using Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.Customer;
using Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.Order;
using Wipcore.Enova.Api.NetClient;

namespace Wipcore.eNova.Api.NETClient
{
    public class NetClientModule : Autofac.Module, IEnovaApiModule
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ApiClient>().AsImplementedInterfaces();
            builder.RegisterType<ApiRepository>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<CartRepository<CartModel, OrderModel>>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<CustomerRepository<CustomerModel, CartModel, OrderModel>>().AsImplementedInterfaces().SingleInstance();
        }

        public int Priority => 0;
    }
}
