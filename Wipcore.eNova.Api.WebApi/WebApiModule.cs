using Autofac;
using Autofac.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Caching;
using Wipcore.Enova.Api.Interfaces;
using Wipcore.Enova.Api.WebApi.Helpers;
using Wipcore.Enova.Api.WebApi.Services;

namespace Wipcore.Enova.Api.WebApi
{
    public class WebApiModule : Autofac.Module, IEnovaApiModule
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(new MemoryCache("ApiCache")).AsSelf().As<ObjectCache>();

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

        }

        public int Priority => 0;
    }
}