using System;
using Serilog.Core;
using Serilog.Events;

namespace MhLabs.SerilogExtensions
{
    public class MemberIdEnrichment : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            var value = LogValueResolver.Resolve(this);
            var property = new LogEventProperty("MemberId", new ScalarValue(value));
            logEvent.AddOrUpdateProperty(property);
        }
    }

    public class ExceptionEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        if (logEvent.Exception == null)
            return;

        var logEventProperty = propertyFactory.CreateProperty("EscapedException", logEvent.Exception.ToString().Replace("\r\n", "\\r\\n"));
        logEvent.AddPropertyIfAbsent(logEventProperty);
    }        
}
}
