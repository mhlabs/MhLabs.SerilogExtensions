using System;
using System.IO;
using System.Linq;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Json;
using Serilog.Parsing;

namespace MhLabs.SerilogExtensions
{
    /// <inheritdoc />
    /// <summary>
    /// This is a mixture of CompactJsonFormatter and RenderedCompactJsonFormatter with the following changes:
    /// - Always prints the MessageTemplate
    /// - Renders Message if there are no serialized token (i.e {@User})
    /// </summary>
    public class MhLabsCompactJsonFormatter: ITextFormatter
    {
        readonly JsonValueFormatter _valueFormatter;
        private readonly MhSensitivePropertyValueFormatter _logEventPropertiesNameFormatter;

        /// <summary>
        /// Construct a <see cref="MhLabsCompactJsonFormatter"/>, optionally supplying a formatter for
        /// <see cref="LogEventPropertyValue"/>s on the event.
        /// </summary>
        /// <param name="valueFormatter">A value formatter, or null.</param>
        public MhLabsCompactJsonFormatter(JsonValueFormatter valueFormatter = null)
        {
            _valueFormatter = valueFormatter ?? new JsonValueFormatter(typeTagName: "$type");
            _logEventPropertiesNameFormatter = new MhSensitivePropertyValueFormatter();
        }

        /// <summary>
        /// Format the log event into the output. Subsequent events will be newline-delimited.
        /// </summary>
        /// <param name="logEvent">The event to format.</param>
        /// <param name="output">The output.</param>
        public void Format(LogEvent logEvent, TextWriter output)
        {
            _logEventPropertiesNameFormatter.Format(logEvent);
            FormatEvent(logEvent, output, _valueFormatter);
            output.WriteLine();
        }

        /// <summary>
        /// Format the log event into the output.
        /// </summary>
        /// <param name="logEvent">The event to format.</param>
        /// <param name="output">The output.</param>
        /// <param name="valueFormatter">A value formatter for <see cref="LogEventPropertyValue"/>s on the event.</param>
        public static void FormatEvent(LogEvent logEvent, TextWriter output, JsonValueFormatter valueFormatter)
        {
            if (logEvent == null) throw new ArgumentNullException(nameof(logEvent));
            if (output == null) throw new ArgumentNullException(nameof(output));
            if (valueFormatter == null) throw new ArgumentNullException(nameof(valueFormatter));

            AppendMessage(logEvent, output);
            AppendMessageTemplate(logEvent, output);
            AppendTimestamp(logEvent, output);
            AppendRenderings(logEvent, output);
            AppendLevel(logEvent, output);

            foreach (var property in logEvent.Properties)
            {
                var name = property.Key;
                if (name.Length > 0 && name[0] == '@')
                {
                    // Escape first '@' by doubling
                    name = '@' + name;
                }

                output.Write(',');
                JsonValueFormatter.WriteQuotedJsonString(name, output);
                output.Write(':');
                valueFormatter.Format(property.Value, output);
            }

            output.Write('}');
        }

        private static void AppendLevel(LogEvent logEvent, TextWriter output)
        {
            if (logEvent.Level != LogEventLevel.Information)
            {
                output.Write(",\"Level\":\"");
                output.Write(logEvent.Level);
                output.Write('\"');
            }
        }

        private static void AppendRenderings(LogEvent logEvent, TextWriter output)
        {
            var tokensWithFormat = logEvent.MessageTemplate.Tokens
                .OfType<PropertyToken>()
                .Where(pt => pt.Format != null);

            // Better not to allocate an array in the 99.9% of cases where this is false
            // ReSharper disable once PossibleMultipleEnumeration
            if (tokensWithFormat.Any())
            {
                output.Write(",\"Renderings\":[");
                var delim = "";
                foreach (var r in tokensWithFormat)
                {
                    output.Write(delim);
                    delim = ",";
                    var space = new StringWriter();
                    r.Render(logEvent.Properties, space);
                    JsonValueFormatter.WriteQuotedJsonString(space.ToString(), output);
                }

                output.Write(']');
            }
        }

        private static void AppendTimestamp(LogEvent logEvent, TextWriter output)
        {
            output.Write(",\"Timestamp\":\"");
            output.Write(logEvent.Timestamp.UtcDateTime.ToString("O"));
            output.Write("\"");
        }

        private static void AppendMessage(LogEvent logEvent, TextWriter output)
        {
            output.Write("{\"Message\":");

            var message = logEvent.MessageTemplate.Render(logEvent.Properties);
            JsonValueFormatter.WriteQuotedJsonString(message, output);
        }

        private static void AppendMessageTemplate(LogEvent logEvent, TextWriter output)
        {
            output.Write(",\"MessageTemplate\":");
            JsonValueFormatter.WriteQuotedJsonString(logEvent.MessageTemplate.Text, output);
        }
    }
}