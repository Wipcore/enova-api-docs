using System;
using System.Linq;
using System.Runtime.Caching;
using Autofac;
using Microsoft.AspNetCore.Http;
using Wipcore.Enova.Api.Abstractions.Interfaces;
using Wipcore.Enova.Api.Abstractions.Internal;
using Wipcore.Enova.Api.WebApi.Controllers;
using Wipcore.Enova.Api.WebApi.Helpers;

namespace Wipcore.Enova.Api.WebApi
{
    public class WebApiModule : Autofac.Module, IEnovaApiModule
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(new MemoryCache("ApiCache")).AsSelf().As<ObjectCache>();
            builder.RegisterType<EnovaApiControllerDependencies>().AsSelf();

            var mapperTypes = ReflectionHelper.GetAllAvailableTypes().Where(x => x.Name.EndsWith("Mapper"));
            foreach (var type in mapperTypes)
            {
                var derivedType = type.GetMostDerivedType();
                builder.RegisterType(derivedType).AsSelf().AsImplementedInterfaces().InstancePerDependency();
            }

            builder.RegisterAssemblyTypes(AppDomain.CurrentDomain.GetAssemblies())
                .Where(x => x.Name.EndsWith("Model")).AsSelf();

            builder.RegisterAssemblyTypes(AppDomain.CurrentDomain.GetAssemblies())
                .Where(x => x.Name.EndsWith("Service")).AsSelf().AsImplementedInterfaces().SingleInstance();

            builder.RegisterType<HttpContextAccessor>().AsSelf().AsImplementedInterfaces().SingleInstance();

        }

        public int Priority => 0;
    }
}