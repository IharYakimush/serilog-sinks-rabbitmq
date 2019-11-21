using System;
using System.Buffers.Text;
using System.Collections.Generic;
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

        private const string ExceptionKey = "exception";

        public override LogEvent Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, LogEvent value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            WriteTimestamp(writer, value);

            WriteLevel(writer, value);

            WriteMessage(writer, value);

            foreach (KeyValuePair<string, LogEventPropertyValue> pair in value.Properties)
            {
                WriteProperty(writer, pair.Key, pair.Value);
            }

            WriteException(writer, value);

            writer.WriteEndObject();
        }

        private void WriteProperty(Utf8JsonWriter writer, string key, LogEventPropertyValue value)
        {
            switch (value)
            {
                case ScalarValue val:
                    WriteScalarValue(writer, key, val);
                    break;
                case SequenceValue val:
                    WriteSequenceValue(writer, key, val);
                    break;
                case StructureValue val:
                    WriteStructureValue(writer, key, val);
                    break;
                case DictionaryValue val:
                    WriteDictionaryValue(writer, key, val);
                    break;
                default: return;
            }
        }

        private void WriteDictionaryValue(Utf8JsonWriter writer, string key, DictionaryValue val)
        {
            
        }

        private void WriteStructureValue(Utf8JsonWriter writer, string key, StructureValue val)
        {
            writer.WriteStartObject(HandleKeyPrefix(this.Options.ObjectKeyPrefix, key));

            if (this.Options.WriteObjectTypeTag && val.TypeTag != null)
            {
                writer.WriteString(this.Options.ObjectTypeTagPropertyName, val.TypeTag);
            }

            foreach (LogEventProperty property in val.Properties)
            {
                WriteProperty(writer, property.Name, property.Value);
            }

            writer.WriteEndObject();
        }

        private void WriteSequenceValue(Utf8JsonWriter writer, string key, SequenceValue val)
        {
            
        }

        private void WriteScalarValue(Utf8JsonWriter writer, string key, ScalarValue value)
        {
            //TODO: handle nullable
            switch (value.Value)
            {
                case string val:
                    writer.WriteString(HandleKeyPrefix(this.Options.StringKeyPrefix, key), val);
                    break;
                case int val:
                    writer.WriteNumber(HandleKeyPrefix(this.Options.IntKeyPrefix, key), val);
                    break;
                case long val:
                    writer.WriteNumber(HandleKeyPrefix(this.Options.LongKeyPrefix, key), val);
                    break;
                case float val:
                    writer.WriteNumber(HandleKeyPrefix(this.Options.FloatKeyPrefix, key), val);
                    break;
                case double val:
                    writer.WriteNumber(HandleKeyPrefix(this.Options.DoubleKeyPrefix, key), val);
                    break;
                case uint val:
                    writer.WriteNumber(HandleKeyPrefix(this.Options.UintKeyPrefix, key), val);
                    break;
                case ulong val:
                    writer.WriteNumber(HandleKeyPrefix(this.Options.UlongKeyPrefix, key), val);
                    break;
                case decimal val:
                    writer.WriteNumber(HandleKeyPrefix(this.Options.DecimalKeyPrefix, key), val);
                    break;
                case DateTimeOffset val:
                    writer.WritePropertyName(HandleKeyPrefix(this.Options.DateTimeOffsetKeyPrefix, key));
                    this.Options.DateTimeOffsetConverter.Write(writer, val, this.Options.JsonOptions);
                    break;
                case DateTime val:
                    writer.WritePropertyName(HandleKeyPrefix(this.Options.DateTimeKeyPrefix, key));
                    this.Options.DateTimeOffsetConverter.Write(writer, val, this.Options.JsonOptions);
                    break;
                default: return;
            }
        }

        private string HandleKeyPrefix(string prefix, string key)
        {
            return prefix == null ? key : prefix + this.Options.JsonOptions.PropertyNamingPolicy.ConvertName(key);
        }

        private void WriteException(Utf8JsonWriter writer, LogEvent value)
        {
            if (this.Options.WriteException && value.Exception != null)
            {
                writer.WriteString(this.Options.ExceptionPropertyName, value.Exception.ToString());
            }
        }

        private void WriteMessage(Utf8JsonWriter writer, LogEvent value)
        {
            if (this.Options.WriteMessage)
            {
                writer.WriteString(this.Options.MessagePropertyName, this.Options.RenderMessage ? value.RenderMessage() : value.MessageTemplate.Text);
            }
        }

        private void WriteLevel(Utf8JsonWriter writer, LogEvent value)
        {
            writer.WritePropertyName(this.Options.LevelPropertyName);

            this.Options.EnumConverter.Write(writer, value.Level, this.Options.JsonOptions);
        }

        private void WriteTimestamp(Utf8JsonWriter writer, LogEvent value)
        {
            //TODO: choose default timestamp format
            //TODO: init converters in constructor
            writer.WritePropertyName(this.Options.TimestampPropertyName);

            this.Options.DateTimeOffsetConverter.Write(writer, value.Timestamp, this.Options.JsonOptions);
        }
    }
}