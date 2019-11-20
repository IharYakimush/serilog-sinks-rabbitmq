using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Serilog.Events;
using Serilog.Formatting.Json;
using Serilog.Parsing;
using Serilog.Sinks.RabbitMq.Client;
using Xunit;
using Xunit.Abstractions;

namespace Serilog.Sinks.RabbitMq.Tests
{
    public class PerfTests
    {
        static Exception exception = new Exception("test");

        static LogEvent logEvent = new LogEvent(DateTimeOffset.Now, LogEventLevel.Debug, exception,
            new MessageTemplate("qwe", new[] { new TextToken("qwe"), }),
            new[]
            {
                    new LogEventProperty("p1", new ScalarValue(1)),
                    new LogEventProperty("p2", new ScalarValue("some Text")),
                    new LogEventProperty("p3", new StructureValue(new[]
                    {
                        new LogEventProperty("p31", new ScalarValue(2)),
                        new LogEventProperty("p32", new ScalarValue("ppp")),
                    })),
                    new LogEventProperty("p4",new SequenceValue(new []
                    {
                        new ScalarValue("11111111111111111111111111111111111111111111111111111111111111"),
                        new ScalarValue("22222222222222222222222222222222222222222222222222222222222222"),
                        new ScalarValue("33333333333333333333333333333333333333333333333333333333333333"),
                        new ScalarValue("44444444444444444444444444444444444444444444444444444444444444"),
                        new ScalarValue("555555555555555555555555555555555555555555555555555555555555555"),
                        new ScalarValue("666666666666666666666666666666666666666666666666666666666666666"),
                        new ScalarValue("777777777777777777777777777777777777777777777777777777777777777"),
                    })),
                    new LogEventProperty("p5",new DictionaryValue(new []
                    {
                        new KeyValuePair<ScalarValue, LogEventPropertyValue>(new ScalarValue("d1"),new ScalarValue("dv1") ),
                        new KeyValuePair<ScalarValue, LogEventPropertyValue>(new ScalarValue("d2"),new ScalarValue("dv2") ),
                        new KeyValuePair<ScalarValue, LogEventPropertyValue>(new ScalarValue("d3"),new ScalarValue("dv3") ),
                    })),
            });

        private readonly ITestOutputHelper output;
        public PerfTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void Utf8Json()
        {
            byte[] result = null;
            JsonToUtf8BytesFormatter formatter = new JsonToUtf8BytesFormatter();
            

            Stopwatch stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < 10000; i++)
            {
                result = formatter.GetBytes(logEvent);
                Assert.True(result.Length > 0);
            }

            this.output.WriteLine(stopwatch.ElapsedMilliseconds.ToString("F"));
            this.output.WriteLine(result.Length.ToString("D"));
        }

        [Fact]
        public void TestFormatter()
        {
            byte[] result = null;
            IBinaryFormatter formatter = new TextToBinaryFormatter(new JsonFormatter(renderMessage: true),
                new TextToBinaryFormatterOptions() {Encoding = Encoding.UTF8});


            Stopwatch stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < 10000; i++)
            {
                result = formatter.GetBytes(logEvent);
                Assert.True(result.Length > 0);
            }

            this.output.WriteLine(stopwatch.ElapsedMilliseconds.ToString("F"));
            this.output.WriteLine(result.Length.ToString("D"));
        }
    }
}