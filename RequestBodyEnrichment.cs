using System;
using Serilog.Core;
using Serilog.Events;

namespace MhLabs.SerilogExtensions
{
    public class RequestBodyEnrichment: ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            var value = LogValueResolver.Resolve(this);
            var property = new LogEventProperty("RequestBody", new ScalarValue(value));
            logEvent.AddOrUpdateProperty(property);
        }
    }
}
