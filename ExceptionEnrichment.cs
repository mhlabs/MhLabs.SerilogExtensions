using Serilog.Core;
using Serilog.Events;

namespace MhLabs.SerilogExtensions
{
    public class ExceptionEnrichment : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            if (logEvent.Exception == null) return;
            
            var type = new LogEventProperty("ExceptionType", new ScalarValue(logEvent.Exception.GetType().Name));
            var value = new LogEventProperty("Exception", new ScalarValue(logEvent.Exception.ToString()));
            logEvent.AddOrUpdateProperty(type);
            logEvent.AddOrUpdateProperty(value);
        }
    }
}