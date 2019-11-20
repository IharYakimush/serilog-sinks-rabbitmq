using System;
using System.Buffers.Text;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using Serilog.Events;
using Serilog.Sinks.RabbitMq.Client;

namespace Serilog.Sinks.RabbitMq.Json
{
    public class LogEventJsonConverter : JsonConverter<LogEvent>
    {
        public LogEventJsonConverterOptions Options { get; }

        public LogEventJsonConverter(LogEventJsonConverterOptions options)
        {
            Options = options ?? throw new ArgumentNullException(nameof(options));
        }

        private const string MessageKey = "message";
        private const string LevelKey = "level";
        private const string TimestampKey = "timestamp";
        private const string ExceptionKey = "exception";

        public override LogEvent Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, LogEvent value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            WriteTimestamp(writer, value, options);

            WriteLevel(writer, value, options);

            WriteMessage(writer, value);

            WriteException(writer, value);

            writer.WriteEndObject();
        }

        private void WriteException(Utf8JsonWriter writer, LogEvent value)
        {
            if (this.Options.WriteException && value.Exception != null)
            {
                writer.WriteString(ExceptionKey, value.Exception.ToString());
            }
        }

        private void WriteMessage(Utf8JsonWriter writer, LogEvent value)
        {
            if (this.Options.WriteMessage)
            {
                writer.WriteString(MessageKey, this.Options.RenderMessage ? value.RenderMessage() : value.MessageTemplate.Text);
            }
        }

        private void WriteLevel(Utf8JsonWriter writer, LogEvent value, JsonSerializerOptions options)
        {
            writer.WritePropertyName(LevelKey);

            this.Options.EnumConverter.Write(writer, value.Level, options);
        }

        private void WriteTimestamp(Utf8JsonWriter writer, LogEvent value, JsonSerializerOptions options)
        {
            //TODO: choose default timestamp format
            //TODO: init converters in constructor
            writer.WritePropertyName(TimestampKey);

            this.Options.DateTimeOffsetConverter.Write(writer, value.Timestamp, options);
        }
    }
}