using SimpleInjector.Lifestyles;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleInjector.Extensions.HealthChecks
{
    internal class HealthCheckService : IHealthCheckService
    {
        private readonly HealthCheckRegistration[] _healthCheckRegistrations;
        private readonly Container _container;

        public HealthCheckService(Container container, HealthCheckRegistration[] healthCheckRegistration)
        {
            _container = container;
            _healthCheckRegistrations = healthCheckRegistration;
        }

        public async Task<HealthReport> CheckHealth(CancellationToken ct = default(CancellationToken), Func<HealthCheckRegistration, bool> predicate = null)
        {
            predicate = predicate ?? (registration => true);

            var totalDuration = TimeSpan.Zero;
            var entries = new Dictionary<string, HealthReportEntry>();

            using (var scope = AsyncScopedLifestyle.BeginScope(_container))
            {
                foreach (var registration in _healthCheckRegistrations.Where(predicate))
                {
                    ct.ThrowIfCancellationRequested();

                    using (var cts = CancellationTokenSource.CreateLinkedTokenSource(ct))
                    {
                        var timeoutTask = GetTimeoutEntry(cts.Token, registration);
                        var checkHealthCoreTask = CheckHealthCore(cts.Token, registration);

                        var entryTask = await Task.WhenAny(timeoutTask, checkHealthCoreTask);

                        var entry = await entryTask;

                        totalDuration += entry.Elapsed;

                        entries[registration.Name] = entry;
                    }
                }
            }

            return new HealthReport(new ReadOnlyDictionary<string, HealthReportEntry>(entries), totalDuration);
        }

        private async Task<HealthReportEntry> CheckHealthCore(CancellationToken ct, HealthCheckRegistration registration)
        {
            var healthCheck = registration.Factory(_container);
            HealthStatus status;
            string description;
            Exception exception;
            IReadOnlyDictionary<string, object> data = null;

            var stopWatch = Stopwatch.StartNew();

            try
            {
                var result = await healthCheck.CheckHealth(ct, new HealthCheckContext
                {
                    Registration = registration,
                    ServiceProvider = _container,
                });

                status = result.Status;
                description = result.Description;
                exception = result.Exception;
                data = result.Data;
            }
            catch (NotImplementedException)
            {
                return new HealthReportEntry(HealthStatus.NotImplemented, "This health check is not implemented.", stopWatch.Elapsed);
            }
            catch (Exception ex) when (!(ex is OperationCanceledException))
            {
                status = registration.FailureStatus;
                description = ex.Message;
                exception = ex;
                data = ex.Data is IDictionary<string, object> dataValues ? new ReadOnlyDictionary<string, object>(dataValues) : default(ReadOnlyDictionary<string, object>);
            }

            stopWatch.Stop();

            return new HealthReportEntry(status, description, stopWatch.Elapsed, exception, data, registration.Tags);
        }

        private async Task<HealthReportEntry> GetTimeoutEntry(CancellationToken ct, HealthCheckRegistration registration)
        {
            await Task.Delay(registration.Timeout ?? Timeout.InfiniteTimeSpan, ct);
            return new HealthReportEntry(
                HealthStatus.TimedOut,
                description: $"The health check for registration {registration} has timed out ({registration.Timeout.Value.TotalMilliseconds} ms).",
                registration.Timeout.Value,
                exception: null,
                data: null,
                tags: null);
        }
    }
}
