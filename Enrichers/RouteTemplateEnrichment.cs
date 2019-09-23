using Serilog.Core;
using Serilog.Events;

namespace MhLabs.SerilogExtensions
{
    public class RouteTemplateEnrichment : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            var property = new LogEventProperty(Constants.RouteTemplateHeader, new ScalarValue(LogValueResolver.Resolve(this)));
            logEvent.AddOrUpdateProperty(property);
        }
    }
}
