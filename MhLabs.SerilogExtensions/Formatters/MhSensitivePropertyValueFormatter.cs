using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MhLabs.SerilogExtensions
{
    /// <summary>
    /// Formatter to mask data based on list of sensitive keywords
    /// </summary>
    public class MhSensitivePropertyValueFormatter
    {
        private readonly List<string> _sensitiveKeyWords = new List<string>
        {
            "password",
            "dateofbirth"
        };

        /// <summary>
        /// Construct a <see cref="MhSensitivePropertyValueFormatter"/>
        /// </summary>
        public MhSensitivePropertyValueFormatter(IEnumerable<string> additionalKeywords = null)
        {
            if (additionalKeywords != null && additionalKeywords.Any())
            {
                _sensitiveKeyWords.AddRange(additionalKeywords);
            }
        }

        /// <summary>
        /// Replace sensitive values with *****
        /// </summary>
        /// <param name="logEvent"></param>
        public void Format(LogEvent logEvent)
        {
            var keys = new List<string>(logEvent.Properties.Keys);

            foreach (var key in keys)
            {
                logEvent.AddOrUpdateProperty(Visit(key, logEvent.Properties[key]));
            }
        }

        private LogEventProperty Visit(string name, LogEventPropertyValue value)
        {
            switch (value)
            {
                case null:
                    throw new ArgumentNullException(nameof(value));
                case ScalarValue scalar:
                    return VisitScalar(name, scalar);
                case SequenceValue sequence:
                    return VisitSequence(name, sequence);
                case StructureValue scalar:
                    return VisitStructure(name, scalar);
                case DictionaryValue dictionary:
                    return VisitDictionary(name, dictionary);
                default:
                    return new LogEventProperty(name, value);
            }
        }

        private LogEventProperty VisitDictionary(string name, DictionaryValue dictionary)
        {
            var formattedElements = new Dictionary<ScalarValue, LogEventPropertyValue>(dictionary.Elements.Count);

            foreach (var element in dictionary.Elements)
            {
                var property = Visit(element.Key.Value.ToString(), element.Value);
                formattedElements.Add(new ScalarValue(property.Name), property.Value);
            }

            return new LogEventProperty(name, new DictionaryValue(formattedElements));
        }

        private LogEventProperty VisitStructure(string name, StructureValue structure)
        {
            var properties = structure.Properties.Select(p => Visit(p.Name, p.Value));
            return new LogEventProperty(name, new StructureValue(properties, structure.TypeTag));
        }

        private LogEventProperty VisitSequence(string name, SequenceValue sequence)
        {
            var elements = sequence.Elements.Select(e => Visit(name, e).Value);
            return new LogEventProperty(name, new SequenceValue(elements));
        }

        private LogEventProperty VisitScalar(string name, ScalarValue scalarValue)
        {
            scalarValue = GetSafeValue(name, scalarValue);
            return new LogEventProperty(name, scalarValue);
        }

        private ScalarValue GetSafeValue(string name, ScalarValue scalarValue)
        {
            if (string.IsNullOrEmpty(scalarValue?.Value?.ToString()))
            {
                return scalarValue;
            }

            if (_sensitiveKeyWords.Contains(name, StringComparer.OrdinalIgnoreCase))
            {
                return new ScalarValue("*****");
            }

            return scalarValue;
        }

    }
}
