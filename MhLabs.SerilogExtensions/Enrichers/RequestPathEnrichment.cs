using Serilog.Core;
using Serilog.Events;

namespace MhLabs.SerilogExtensions
{
    public class RequestPathEnrichment : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            var value = LogValueResolver.Resolve(this);
            if (value == default(string)) return; // don't set null values

            var property = new LogEventProperty("RequestPath", new ScalarValue(value));
            logEvent.AddOrUpdateProperty(property);
        }
    }
}