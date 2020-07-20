using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleInjector.Extensions.HealthChecks
{
    public interface IHealthCheckBuilder
    {
        IHealthCheckBuilder Register(
            string name,
            HealthCheckDelegate healthCheck,
            HealthStatus failureStatus = HealthStatus.Unhealthy,
            IEnumerable<string> tags = null,
            TimeSpan? timeout = null);

        IHealthCheckBuilder Register<THealthCheck>(
            string name = null,
            HealthStatus failureStatus = HealthStatus.Unhealthy,
            IEnumerable<string> tags = null,
            TimeSpan? timeout = null)
                where THealthCheck : IHealthCheck;

        IHealthCheckBuilder RegisterFromInterface<THealthCheck>(Assembly assembly) where THealthCheck : IHealthCheck;
    }
}