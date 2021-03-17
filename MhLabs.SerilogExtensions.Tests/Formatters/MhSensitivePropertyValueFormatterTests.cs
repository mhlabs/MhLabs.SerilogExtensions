using Serilog.Events;
using Serilog.Parsing;
using System.Collections.Generic;
using Xunit;

namespace MhLabs.SerilogExtensions.Tests.Formatters
{
    public class MhSensitivePropertyValueFormatterTests
    {
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
            var _formatter = new MhSensitivePropertyValueFormatter();

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


        [Theory]
        [InlineData("secret", "abc", "secret", "*****")]
        [InlineData("SECRET", "abc", "secret", "*****")]
        [InlineData("password", "abc", "secret", "*****")]
        public void Test_AdditionalKeywords(string name, string value, string keyWordToAdd, string expected)
        {
            // arrange
            var scalar = new ScalarValue(value);
            var logEventProperty = new LogEventProperty(name, scalar);
            var structureValue = new StructureValue(new List<LogEventProperty> { logEventProperty });
            var _formatter = new MhSensitivePropertyValueFormatter(new List<string> { keyWordToAdd });
            

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
