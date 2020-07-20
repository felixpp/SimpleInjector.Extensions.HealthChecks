using System;

namespace SimpleInjector.Extensions.HealthChecks
{
    public class HealthCheckContext
    {
        public HealthCheckRegistration Registration { get; set; }

        public IServiceProvider ServiceProvider { get; set; }
    }
}