using System;
using Serilog.Core;
using Serilog.Events;

namespace MhLabs.SerilogExtensions
{
    public class XRayEnrichment : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            var property = new LogEventProperty("XRayTraceId", new ScalarValue(System.Environment.GetEnvironmentVariable("_X_AMZN_TRACE_ID")));
            logEvent.AddOrUpdateProperty(property);
        }
    }
}
