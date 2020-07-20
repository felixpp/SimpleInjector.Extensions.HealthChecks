using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleInjector.Extensions.HealthChecks
{
    public interface IHealthCheck
    {
        Task<HealthCheckResult> CheckHealth(CancellationToken ct, HealthCheckContext context);
    }
}
