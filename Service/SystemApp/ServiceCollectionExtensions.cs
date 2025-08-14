using Domain.Attributes;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Service.SystemApp
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddServicesWithAttributes(
            this IServiceCollection services,
            params Assembly[] assemblies)
        {
            var types = assemblies
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsClass && !t.IsAbstract)
                .Select(t => new
                {
                    Implementation = t,
                    Attribute = t.GetCustomAttribute<RegisterServiceAttribute>()
                })
                .Where(x => x.Attribute != null);

            foreach (var type in types)
            {
                var attr = type.Attribute!;

                if (attr.ServiceType != null)
                {
                    services.Add(new ServiceDescriptor(attr.ServiceType, type.Implementation, attr.Lifetime));
                }
                else if (attr.RegisterAllInterfaces)
                {
                    foreach (var iface in type.Implementation.GetInterfaces())
                    {
                        services.Add(new ServiceDescriptor(iface, type.Implementation, attr.Lifetime));
                    }
                }
                if (attr.AsSelf)
                {
                    services.Add(new ServiceDescriptor(type.Implementation, type.Implementation, attr.Lifetime));
                }
                else if (type.Implementation.GetInterfaces().Any() && !attr.RegisterAllInterfaces && attr.ServiceType == null)
                {
                    var firstIface = type.Implementation.GetInterfaces().First();
                    services.Add(new ServiceDescriptor(firstIface, type.Implementation, attr.Lifetime));
                }
            }

            return services;
        }
    }
}
