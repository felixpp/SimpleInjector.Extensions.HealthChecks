using System;
using System.Collections.Generic;

namespace SimpleInjector.Extensions.HealthChecks
{
    public class HealthCheckRegistration
    {
        private readonly Func<IServiceProvider, IHealthCheck> _healthCheckProvider;
        
        public HealthCheckRegistration(string name, Func<IServiceProvider, IHealthCheck> healthCheckProvider, HealthStatus failureStatus = HealthStatus.Unhealthy, IEnumerable<string> tags = null, TimeSpan? timeout = null)
        {
            _healthCheckProvider = healthCheckProvider ?? throw new ArgumentNullException(nameof(healthCheckProvider));
            
            Name = name;
            FailureStatus = failureStatus;
            Tags = tags;
            Timeout = timeout;
        }

        public string Name { get; }

        public TimeSpan? Timeout { get; }

        public IEnumerable<string> Tags { get; }

        public HealthStatus FailureStatus { get; }

        public IHealthCheck Factory(IServiceProvider serviceProvider) => _healthCheckProvider(serviceProvider);
    }
}