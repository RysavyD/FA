using System;
using Autofac;

namespace _3F.Web
{
    public static class ContainerBuilderExtensions
    {
        public static void RegisterFromAppSettings<T>(this ContainerBuilder builder, bool perRequest = true)
        {
            var value = System.Configuration.ConfigurationManager.AppSettings[typeof(T).Name];
            if (perRequest)
                builder.RegisterType(Type.GetType(value)).As<T>().InstancePerRequest();
            else
                builder.RegisterType(Type.GetType(value)).As<T>();
        }
    }
}