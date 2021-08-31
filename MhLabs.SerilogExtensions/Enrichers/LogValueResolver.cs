using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Serilog.Core;

namespace MhLabs.SerilogExtensions
{
    public static class LogValueResolver
    {
        public static ConcurrentDictionary<string, Func<string>> _values = new ConcurrentDictionary<string, Func<string>>();
        public static void Register<T>(Func<string> value) where T : ILogEventEnricher
        {
            var key = typeof(T).Name;
            if (_values.ContainsKey(key))
            {
                _values[key] = value;
            }
            else
            {
                _values.TryAdd(key, value);
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
