using Serilog.Core;
using Serilog.Events;

namespace MhLabs.SerilogExtensions
{
    public class UserAudienceEnrichment : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            var value = LogValueResolver.Resolve(this);
            var property = new LogEventProperty("UserAudience", new ScalarValue(value));
            logEvent.AddOrUpdateProperty(property);
        }
    }
}