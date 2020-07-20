using System;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleInjector.Extensions.HealthChecks
{
    public interface IHealthCheckService
    {
        Task<HealthReport> CheckHealth(CancellationToken ct = default(CancellationToken), Func<HealthCheckRegistration, bool> predicate = null);
    }
}