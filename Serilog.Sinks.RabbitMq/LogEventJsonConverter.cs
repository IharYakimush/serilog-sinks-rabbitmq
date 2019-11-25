using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.ObjectPool;
using Serilog.Events;
using Serilog.Sinks.RabbitMq.Client;

namespace Serilog.Sinks.RabbitMq.Json
{
    public class LogEventJsonConverter : JsonConverter<LogEvent>
    {
        private readonly ObjectPool<StringBuilder> stringBuilders =
            new DefaultObjectPoolProvider().CreateStringBuilderPool(100, 5);
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
                WriteProperty(writer, pair.Key, pair.Value,false);
            }

            WriteException(writer, value);

            writer.WriteEndObject();
        }

        private bool WriteProperty(Utf8JsonWriter writer, string key, LogEventPropertyValue value, bool array)
        {
            switch (value)
            {
                case ScalarValue val:
                    return WriteScalarValue(writer, key, val, array);
                case SequenceValue val:
                    WriteSequenceValue(writer, key, val);
                    break;
                case StructureValue val:
                    WriteStructureValue(writer, key, val, array);
                    break;
                case DictionaryValue val:
                    WriteDictionaryValue(writer, key, val, array);
                    break;
                default: return false;
            }

            return true;
        }

        private void WriteDictionaryValue(Utf8JsonWriter writer, string key, DictionaryValue val, bool array)
        {
            if (key != null)
            {
                writer.WritePropertyName(HandleKeyPrefix(this.Options.ObjectKeyPrefix, key, array));
                if (array)
                {
                    writer.WriteStartArray();
                }
            }

            writer.WriteStartObject();

            foreach (KeyValuePair<ScalarValue, LogEventPropertyValue> kvp in val.Elements)
            {
                if (kvp.Key.Value is string itemKey)
                {
                    WriteProperty(writer, itemKey, kvp.Value, false);
                }
            }

            writer.WriteEndObject();
        }

        private void WriteStructureValue(Utf8JsonWriter writer, string key, StructureValue val, bool array)
        {
            if (key != null)
            {
                writer.WritePropertyName(HandleKeyPrefix(this.Options.ObjectKeyPrefix, key, array));
                if (array)
                {
                    writer.WriteStartArray();
                }
            }

            writer.WriteStartObject();

            if (this.Options.WriteObjectTypeTag && val.TypeTag != null)
            {
                writer.WriteString(this.Options.ObjectTypeTagPropertyName, val.TypeTag);
            }

            foreach (LogEventProperty property in val.Properties)
            {
                WriteProperty(writer, property.Name, property.Value, false);
            }

            writer.WriteEndObject();
        }

        private void WriteSequenceValue(Utf8JsonWriter writer, string key, SequenceValue val)
        {
            bool started = false;

            IReadOnlyList<LogEventPropertyValue> values = val.Elements;

            for (int i = 0; i < values.Count; i++)
            {
                if (started)
                {
                    WriteProperty(writer, null, values[i], true);
                }
                else
                {
                    started = WriteProperty(writer, key, values[i], true);
                }
            }

            if (started)
            {
                writer.WriteEndArray();
            }
        }

        private bool WriteScalarValue(Utf8JsonWriter writer, string key, ScalarValue value, bool array)
        {
            switch (value.Value)
            {
                case string val:
                    if (key != null)
                    {
                        writer.WritePropertyName(HandleKeyPrefix(this.Options.StringKeyPrefix, key, array)); 
                        if (array) writer.WriteStartArray();
                    }
                    writer.WriteStringValue(val);
                    break;
                case int val:
                    if (key != null)
                    {
                        writer.WritePropertyName(HandleKeyPrefix(this.Options.IntKeyPrefix, key, array)); 
                        if (array) writer.WriteStartArray();
                    }
                    writer.WriteNumberValue(val);
                    break;
                case long val:
                    if (key != null)
                    {
                        writer.WritePropertyName(HandleKeyPrefix(this.Options.LongKeyPrefix, key, array));
                        if (array) writer.WriteStartArray();
                    }
                    writer.WriteNumberValue(val);
                    break;
                case float val:
                    if (key != null)
                    {
                        writer.WritePropertyName(HandleKeyPrefix(this.Options.FloatKeyPrefix, key, array)); 
                        if (array) writer.WriteStartArray();
                    }
                    writer.WriteNumberValue(val);
                    break;
                case double val:
                    if (key != null)
                    {
                        writer.WritePropertyName(HandleKeyPrefix(this.Options.DoubleKeyPrefix, key, array));
                        if (array) writer.WriteStartArray();
                    }
                    writer.WriteNumberValue(val);
                    break;
                case uint val:
                    if (key != null)
                    {
                        writer.WritePropertyName(HandleKeyPrefix(this.Options.UintKeyPrefix, key, array)); 
                        if (array) writer.WriteStartArray();
                    }
                    writer.WriteNumberValue(val);
                    break;
                case ulong val:
                    if (key != null)
                    {
                        writer.WritePropertyName(HandleKeyPrefix(this.Options.UlongKeyPrefix, key, array)); 
                        if (array) writer.WriteStartArray();
                    }
                    writer.WriteNumberValue(val);
                    break;
                case decimal val:
                    if (key != null)
                    {
                        writer.WritePropertyName(HandleKeyPrefix(this.Options.DecimalKeyPrefix, key, array)); 
                        if (array) writer.WriteStartArray();
                    }
                    writer.WriteNumberValue(val);
                    break;
                case DateTimeOffset val:
                    if (key != null)
                    {
                        writer.WritePropertyName(HandleKeyPrefix(this.Options.DateTimeOffsetKeyPrefix, key, array)); 
                        if (array) writer.WriteStartArray();
                    }
                    this.Options.DateTimeOffsetConverter.Write(writer, val, this.Options.JsonOptions);
                    break;
                case DateTime val:
                    if (key != null)
                    {
                        writer.WritePropertyName(HandleKeyPrefix(this.Options.DateTimeKeyPrefix, key, array)); 
                        if (array) writer.WriteStartArray();
                    }
                    this.Options.DateTimeConverter.Write(writer, val, this.Options.JsonOptions);
                    break;
                default: return false;
            }

            return true;
        }

        private string HandleKeyPrefix(string keyPrefix, string key, bool array)
        {
            if (keyPrefix == null && (this.Options.ArrayKeyPrefix == null || !array))
            {
                return key;
            }

            StringBuilder sb = this.stringBuilders.Get();

            if (array && this.Options.ArrayKeyPrefix != null)
            {
                sb.Append(this.Options.ArrayKeyPrefix);
            }

            if (keyPrefix != null)
            {
                sb.Append(keyPrefix);
            }

            if (this.Options.JsonOptions.PropertyNamingPolicy == null)
            {
                sb.Append(key);
            }
            else
            {
                sb.Append(this.Options.JsonOptions.PropertyNamingPolicy.ConvertName(key));
            }

            string result = sb.ToString();
            this.stringBuilders.Return(sb);

            return result;
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