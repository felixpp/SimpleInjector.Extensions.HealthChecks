using System;
using System.Reflection;

namespace SimpleInjector.Extensions.HealthChecks
{
    public static class ContainerExtensions
    {
        public static void RegisterHealthChecks(this Container container, Action<IHealthCheckBuilder> build = null)
        {
            Assembly assembly;
         
            if (build == null)
            {
                assembly = Assembly.GetCallingAssembly();
                build = b => b.RegisterFromInterface<IHealthCheck>(assembly);
            }

            container.Register<IHealthCheckService>(() =>
            {
                var builder = new HealthCheckBuilder(container);
                build(builder);
                return builder.Build();
            });
        }

        public static void RegisterHealthChecks(this Container container, Assembly assembly)
        {
            container.RegisterHealthChecks(b => b.RegisterFromInterface<IHealthCheck>(assembly));
        }
    }
}