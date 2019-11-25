using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using Serilog.Events;
using Serilog.Parsing;
using Serilog.Sinks.RabbitMq.Client;
using Serilog.Sinks.RabbitMq.Json;
using Xunit;
using Xunit.Abstractions;

namespace Serilog.Sinks.RabbitMq.Tests
{
    public class JsonToUtf8BytesFormatterTests
    {
        private ITestOutputHelper output;

        public JsonToUtf8BytesFormatterTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        static List<LogEventProperty> o = new List<LogEventProperty>
        {
            new LogEventProperty("ps1", new ScalarValue("s1")),
        };

        static List<LogEventProperty> o1 = new List<LogEventProperty>
        {
            new LogEventProperty("ps1", new ScalarValue("s1")),
            new LogEventProperty("pi1", new ScalarValue(1)),
            new LogEventProperty("pl1", new ScalarValue(2L)),
            new LogEventProperty("pf1", new ScalarValue(3.4f)),
            new LogEventProperty("pd1", new ScalarValue(3.5d)),
            new LogEventProperty("pui1", new ScalarValue((uint) 4)),
            new LogEventProperty("pul1", new ScalarValue((ulong) 4)),
            new LogEventProperty("pdc1", new ScalarValue((decimal) 5.8)),
            new LogEventProperty("pdt1", new ScalarValue(new DateTime(2019, 9, 23))),
            new LogEventProperty("pdto1", new ScalarValue(new DateTimeOffset(2019, 9, 23, 5, 56, 23, TimeSpan.FromMinutes(1)))),
        };

        static List<LogEventProperty> o2 = new List<LogEventProperty>
        {
            new LogEventProperty("a1", new SequenceValue(new []{ new ScalarValue("s1"),new ScalarValue("s2")})),
            new LogEventProperty("a2", new SequenceValue(new ScalarValue [0])),
            new LogEventProperty("a3", new SequenceValue(new []{ new ScalarValue(null)})),
            new LogEventProperty("a4", new SequenceValue(new []{ new ScalarValue("s1")})),
            new LogEventProperty("a5", new SequenceValue(new []{ new StructureValue(o), new StructureValue(o) })),

        };

        private static List<DictionaryValue> o3 = new List<DictionaryValue>
        {
            new DictionaryValue(new Dictionary<ScalarValue, LogEventPropertyValue>
            {
                {new ScalarValue("ps1"), new ScalarValue("s1")},
                {new ScalarValue("po1"), new StructureValue(o)},
                {new ScalarValue("psq1"), new SequenceValue(new []{ new ScalarValue("s1"),new ScalarValue("s2")})},
            }),
        };

        private static List<LogEventProperty> fo = o1.Concat(o2).Concat(new List<LogEventProperty>
        {
            new LogEventProperty("o1", new StructureValue(o1)),
            new LogEventProperty("o2", new StructureValue(o2)),
            new LogEventProperty("o3", o3[0]),
        }).ToList();

        [Fact]
        public void ConvertWithPrefixes()
        {
            JsonToUtf8BytesFormatter formatter = new JsonToUtf8BytesFormatter(new LogEventJsonConverterOptions()
                {JsonOptions = new JsonSerializerOptions {WriteIndented = true}});

            var bytes = formatter.GetBytes(new LogEvent(DateTimeOffset.Now, LogEventLevel.Warning,
                new Exception("test"), new MessageTemplate("qwe", new MessageTemplateToken[0]), fo));

            Assert.True(bytes.Length > 0);
            this.output.WriteLine(Encoding.UTF8.GetString(bytes));
        }

        [Fact]
        public void ConvertWithoutPrefixes()
        {
            JsonToUtf8BytesFormatter formatter = new JsonToUtf8BytesFormatter(new LogEventJsonConverterOptions()
            {
                JsonOptions = new JsonSerializerOptions {WriteIndented = true},
                ObjectKeyPrefix = null,
                StringKeyPrefix = null,
                DoubleKeyPrefix = null,
                ArrayKeyPrefix = null,
                IntKeyPrefix = null,
                UintKeyPrefix = null,
                UlongKeyPrefix = null,
                LongKeyPrefix = null,
                FloatKeyPrefix = null,
                DateTimeOffsetKeyPrefix = null,
                DateTimeKeyPrefix = null,
                DecimalKeyPrefix = null,
            });

            var bytes = formatter.GetBytes(new LogEvent(DateTimeOffset.Now, LogEventLevel.Warning,
                new Exception("test"), new MessageTemplate("qwe", new MessageTemplateToken[0]), fo));

            Assert.True(bytes.Length > 0);
            this.output.WriteLine(Encoding.UTF8.GetString(bytes));
        }
    }
}