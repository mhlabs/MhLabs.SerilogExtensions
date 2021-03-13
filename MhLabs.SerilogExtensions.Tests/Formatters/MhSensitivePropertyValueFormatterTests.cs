using Serilog.Events;
using Serilog.Parsing;
using System.Collections.Generic;
using Xunit;

namespace MhLabs.SerilogExtensions.Tests.Formatters
{
    public class MhSensitivePropertyValueFormatterTests
    {
        private readonly MhSensitivePropertyValueFormatter _formatter;
        public MhSensitivePropertyValueFormatterTests()
        {
            _formatter = new MhSensitivePropertyValueFormatter();
        }

        [Theory]
        [InlineData("password", "abc", "*****")]
        [InlineData("PASSWORD", "abc", "*****")]
        [InlineData("SafeProperty", "abc", "abc")]
        [InlineData("SafeProperty", "", "")]
        [InlineData("SafeProperty", null, null)]
        public void Test_Format(string name, string value, string expected)
        {
            // arrange
            var scalar = new ScalarValue(value);
            var logEventProperty = new LogEventProperty(name, scalar);

            var structureValue = new StructureValue(new List<LogEventProperty> { logEventProperty });
            

            var logEvent = new LogEvent(
                new System.DateTimeOffset(),
                LogEventLevel.Information,
                null,
                new MessageTemplate("", new List<MessageTemplateToken>()),
                new List<LogEventProperty> { new LogEventProperty("BaseModel", structureValue) });

            // act
            _formatter.Format(logEvent);
           
            var baseModel = logEvent.Properties["BaseModel"] as StructureValue;
            var result = baseModel.Properties[0];

            // assert
            Assert.Equal(name, result.Name);
            Assert.Equal(new ScalarValue(expected), result.Value);
        }

    }
}
