using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SimpleInjector.Extensions.HealthChecks
{
    internal class HealthCheckBuilder : IHealthCheckBuilder
    {
        private readonly List<HealthCheckRegistration> _healthCheckRegistrations = new List<HealthCheckRegistration>();
        private readonly Container _container;

        public HealthCheckBuilder(Container container)
        {
            _container = container;
        }

        public IHealthCheckBuilder Register(
            string name,
            HealthCheckDelegate healthCheck,
            HealthStatus failureStatus = HealthStatus.Unhealthy,
            IEnumerable<string> tags = null,
            TimeSpan? timeout = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new InvalidOperationException("Please provide a name for this health check");
            }

            var registration = new HealthCheckRegistration(name, _ => new DelegatingHealthCheck(healthCheck), failureStatus, tags, timeout);

            _healthCheckRegistrations.Add(registration);

            return this;
        }

        public IHealthCheckBuilder Register<THealthCheck>(
            string name = null,
            HealthStatus failureStatus = HealthStatus.Unhealthy,
            IEnumerable<string> tags = null,
            TimeSpan? timeout = null) where THealthCheck : IHealthCheck
        {
            return Register(
                typeof(THealthCheck),
                string.IsNullOrWhiteSpace(name) ? typeof(THealthCheck).Name : name,
                failureStatus,
                tags,
                timeout);
        }

        public IHealthCheckBuilder Register(
            Type type,
            string name = null,
            HealthStatus failureStatus = HealthStatus.Unhealthy,
            IEnumerable<string> tags = null,
            TimeSpan? timeout = null)
        {
            return Register(
                string.IsNullOrWhiteSpace(name) ? type.Name : name,
                async (ct, context) =>
                {
                    try
                    {
                        var healthCheck = _container.GetInstance(type) as IHealthCheck;
                        return await healthCheck.CheckHealth(ct, context);
                    }
                    catch (ActivationException ex)
                    {
                        return HealthCheckResult.Unhealthy($"Unable to resolve service of type {type.Name} to perform health check.", ex);
                    }
                },
                failureStatus,
                tags,
                timeout);
        }

        public IHealthCheckBuilder RegisterFromInterface<THealthCheck>(Assembly callingAssembly) where THealthCheck : IHealthCheck
        {
            var healthCheckTypes = callingAssembly
                .GetTypes()
                .Where(type => typeof(IHealthCheck).IsAssignableFrom(type) && !type.IsAbstract);

            foreach (var healthCheckType in healthCheckTypes)
            {
                Register(healthCheckType);
            }
            
            return this;
        }

        public HealthCheckService Build()
        {
            return new HealthCheckService(_container, _healthCheckRegistrations.ToArray());
        }
    }
}