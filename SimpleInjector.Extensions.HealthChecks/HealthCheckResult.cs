using System;
using System.Collections.Generic;

namespace SimpleInjector.Extensions.HealthChecks
{
    public struct HealthCheckResult
    {
        public static HealthCheckResult Degraded(string description = null, Exception exception = null, IReadOnlyDictionary<string, object> data = null)
        {
            return new HealthCheckResult(HealthStatus.Degraded, description, exception, data);
        }

        public static HealthCheckResult Healthy(string description = null, IReadOnlyDictionary<string, object> data = null)
        {
            return new HealthCheckResult(HealthStatus.Healthy, description, data: data);
        }

        public static HealthCheckResult Unhealthy(string description = null, Exception exception = null, IReadOnlyDictionary<string, object> data = null)
        {
            return new HealthCheckResult(HealthStatus.Unhealthy, description, exception, data);
        }

        public static HealthCheckResult Inconclusive(string description = null, Exception exception = null, IReadOnlyDictionary<string, object> data = null)
        {
            return new HealthCheckResult(HealthStatus.Inconclusive, description, data: data);
        }

        public static HealthCheckResult NotImplemented()
        {
            return new HealthCheckResult(HealthStatus.NotImplemented, "This health check is not implemented.");
        }

        public HealthCheckResult(HealthStatus status, string description = null, Exception exception = null, IReadOnlyDictionary<string, object> data = null)
        {
            Status = status;
            Description = description;
            Exception = exception;
            Data = data;
        }

        public IReadOnlyDictionary<string, object> Data { get; }

        public string Description { get; }

        public Exception Exception { get; }

        public HealthStatus Status { get; }
    }
}