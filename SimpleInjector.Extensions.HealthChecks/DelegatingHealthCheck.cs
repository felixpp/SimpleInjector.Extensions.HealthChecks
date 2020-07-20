using System;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleInjector.Extensions.HealthChecks
{
    public delegate Task<HealthCheckResult> HealthCheckDelegate(CancellationToken ct, HealthCheckContext context);

    internal class DelegatingHealthCheck : IHealthCheck
    {
        private readonly HealthCheckDelegate _healthCheck;

        public DelegatingHealthCheck(HealthCheckDelegate healthCheck)
        {
            _healthCheck = healthCheck ?? throw new ArgumentNullException();
        }

        public async Task<HealthCheckResult> CheckHealth(CancellationToken ct, HealthCheckContext context)
        {
            return await _healthCheck(ct, context);
        }
    }
}