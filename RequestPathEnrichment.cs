using Serilog.Core;
using Serilog.Events;

namespace MhLabs.SerilogExtensions
{
    public class RequestPathEnrichment : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            var value = LogValueResolver.Resolve(this);
            var property = new LogEventProperty("RequestPath", new ScalarValue(value));
            logEvent.AddOrUpdateProperty(property);
        }
    }
}