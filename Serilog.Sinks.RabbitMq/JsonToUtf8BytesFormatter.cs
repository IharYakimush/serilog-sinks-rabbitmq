using System;
using System.Text.Json;
using Serilog.Events;
using Serilog.Sinks.RabbitMq.Json;

namespace Serilog.Sinks.RabbitMq.Client
{
    public class JsonToUtf8BytesFormatter : IBinaryFormatter
    {
        private readonly LogEventJsonConverterOptions options;

        public JsonToUtf8BytesFormatter(LogEventJsonConverterOptions options = null)
        {
            this.options = options ?? new LogEventJsonConverterOptions();
            this.options.JsonOptions.Converters.Insert(0, new LogEventJsonConverter(this.options));
        }
        public byte[] GetBytes(LogEvent logEvent)
        {
            return JsonSerializer.SerializeToUtf8Bytes(logEvent, this.options.JsonOptions);
        }
    }
}
