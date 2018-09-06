using System;
using System.Text;
using Serilog.Core;
using Serilog.Events;

namespace MhLabs.SerilogExtensions
{
    public class RequestBodyEnrichment : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            var value = LogValueResolver.Resolve(this);
            if (!string.IsNullOrWhiteSpace(value))
            {
                var bytes = Convert.FromBase64String(value);
                var body = Encoding.UTF8.GetString(bytes);

                var property = new LogEventProperty("RequestBody", new ScalarValue(body));
                logEvent.AddOrUpdateProperty(property);
            }
        }
    }
}
