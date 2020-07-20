using System;
using System.Collections.Generic;

namespace SimpleInjector.Extensions.HealthChecks
{
    public class HealthReportEntry
    {
        public HealthReportEntry(HealthStatus status, string description, TimeSpan elapsed, Exception exception = null, IReadOnlyDictionary<string, object> data = null, IEnumerable<string> tags = null)
        {
            Status = status;
            Description = description;
            Elapsed = elapsed;
            Exception = exception;
            Data = data;
            Tags = tags;
        }

        public HealthStatus Status { get; }

        public string Description { get; }

        public TimeSpan Elapsed { get; }

        public Exception Exception { get; }

        public IReadOnlyDictionary<string, object> Data { get; }

        public IEnumerable<string> Tags { get; }
    }
}