namespace SimpleInjector.Extensions.HealthChecks
{
    public enum HealthStatus
    {
        Healthy,
        Degraded,
        Unhealthy,
        Inconclusive,
        TimedOut,
        NotImplemented,
    }
}