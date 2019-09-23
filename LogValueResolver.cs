using System;
using System.Collections.Generic;
using Serilog.Core;
using Serilog.Events;

namespace MhLabs.SerilogExtensions
{
    public static class LogValueResolver
    {
        public static IDictionary<string, Func<string>> _values = new Dictionary<string, Func<string>>();
        public static void Register<T>(Func<string> value) where T : ILogEventEnricher
        {
            var key = typeof(T).Name;
            if (_values.ContainsKey(key))
            {
                _values[key] = value;
            }
            else
            {
                _values.Add(key, value);
            }
        }

        internal static string Resolve(ILogEventEnricher enricher)
        {

            var key = enricher.GetType().Name;
            if (_values.ContainsKey(key))
            {
                return _values[key]();
            }
            return null;
        }

        public static void Clear()
        {
            _values.Clear();
        }
    }
}
