using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleInjector.Extensions.HealthChecks
{
    public class HealthReport
    {
        public static HealthStatus DefaultAggregateStatuses(IEnumerable<HealthReportEntry> entries)
        {
            var firstEntryStatus = entries.FirstOrDefault()?.Status ?? HealthStatus.NotImplemented;
            if (entries.All(entry => entry.Status == firstEntryStatus))
            {
                return firstEntryStatus;
            }
            else if (entries.Any(entry => entry.Status == HealthStatus.Unhealthy))
            {
                return HealthStatus.Unhealthy;
            }
            else if (entries.Any(entry => entry.Status == HealthStatus.Degraded))
            {
                return HealthStatus.Degraded;
            }
            else
            {
                return HealthStatus.Healthy;
            }
        }

        public static Func<IEnumerable<HealthReportEntry>, HealthStatus> AggregateStatuses { get; set; } = DefaultAggregateStatuses;

        public HealthReport(IReadOnlyDictionary<string, HealthReportEntry> entries, TimeSpan totalDuration)
        {
            Entries = entries;
            TotalDuration = totalDuration;
            Status = (AggregateStatuses ?? DefaultAggregateStatuses).Invoke(entries.Values);
        }

        public IReadOnlyDictionary<string, HealthReportEntry> Entries { get; }
        
        public TimeSpan TotalDuration { get; }

        public HealthStatus Status { get; }
    }
}