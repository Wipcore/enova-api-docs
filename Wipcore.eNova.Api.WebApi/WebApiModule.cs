using Autofac;
using Autofac.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Caching;
using Wipcore.eNova.Api.WebApi.Helpers;
using Wipcore.eNova.Api.WebApi.Services;

namespace Wipcore.eNova.Api.WebApi
{
    public class WebApiModule : Autofac.Module
    {
        //public string ProductClass { get; set; }
        //public string SectionClass { get; set; }
        //public string CustomerClass { get; set; }
        
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(new MemoryCache("ApiCache")).AsSelf().As<ObjectCache>();

            IEnumerable<Type> mapperTypes = ReflectionHelper.GetAllAvailableTypes().Where(x => x.Name.EndsWith("Mapper"));
            foreach (Type type in mapperTypes)
            {
                Type type2 = ReflectionHelper.GetMostDerivedType(type);
                //builder.RegisterType(type2).AsSelf().AsBaseTypes().AsImplementedInterfaces().InstancePerDependency();
                builder.RegisterType(type2).AsSelf().AsImplementedInterfaces().InstancePerDependency();
            }

            builder.RegisterAssemblyTypes(AppDomain.CurrentDomain.GetAssemblies())
                    .Where(x => x.Name.EndsWith("Model"))
                    .AsSelf();

            builder.RegisterAssemblyTypes(AppDomain.CurrentDomain.GetAssemblies())
                    .Where(x => x.Name.EndsWith("Service"))
                    .AsSelf()
                    .AsImplementedInterfaces();
            
            //this.RegisterTypeByName(builder, ProductClass);
            //this.RegisterTypeByName(builder, SectionClass);
            //this.RegisterTypeByName(builder, CustomerClass);

            //builder.RegisterType(typeof(EnovaAttributeType).GetMostDerivedType(ReflectionHelper.GetAllAvailableTypes())).AsSelf().AsBaseTypes().AsImplementedInterfaces().InstancePerDependency();
            //builder.RegisterType(typeof(EnovaAttributeTypeGroup).GetMostDerivedType(ReflectionHelper.GetAllAvailableTypes())).AsSelf().AsBaseTypes().AsImplementedInterfaces().InstancePerDependency();
            //builder.RegisterType(typeof(EnovaAttributeValue).GetMostDerivedType(ReflectionHelper.GetAllAvailableTypes())).AsSelf().AsBaseTypes().AsImplementedInterfaces().InstancePerDependency();

            //// Register from all loaded assemblies in order to register plugin custom controllers
            //builder.RegisterApiControllers(AppDomain.CurrentDomain.GetAssemblies());
        }

        private void RegisterTypeByName(ContainerBuilder builder, string typeName)
        {
            //Type type = Type.GetType(typeName, true);
            //builder.RegisterType(type).AsSelf().AsBaseTypes().AsImplementedInterfaces().InstancePerDependency();
        }
    }
}