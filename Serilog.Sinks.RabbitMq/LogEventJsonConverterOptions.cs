using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Serilog.Sinks.RabbitMq.Client.JsonConverters;

namespace Serilog.Sinks.RabbitMq.Client
{
    public class LogEventJsonConverterOptions
    {
        public JsonSerializerOptions JsonOptions { get; set; } = new JsonSerializerOptions
        {
            IgnoreNullValues = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public bool WriteException { get; set; } = true;
        public bool WriteObjectTypeTag { get; set; } = true;
        public bool WriteMessage { get; set; } = true;
        public bool RenderMessage { get; set; } = true;

        public string TimestampPropertyName { get; set; } = "timestamp";
        public string LevelPropertyName { get; set; } = "level";
        public string MessagePropertyName { get; set; } = "message";
        public string ExceptionPropertyName { get; set; } = "exception";
        public string ObjectTypeTagPropertyName { get; set; } = "_type";

        public string DateTimeOffsetKeyPrefix { get; set; } = "dto_";
        public string DateTimeKeyPrefix { get; set; } = "dt_";
        public string StringKeyPrefix { get; set; } = "s_";
        public string IntKeyPrefix { get; set; } = "i_";
        public string LongKeyPrefix { get; set; } = "i_";
        public string FloatKeyPrefix { get; set; } = "f_";
        public string DoubleKeyPrefix { get; set; } = "f_";
        public string UintKeyPrefix { get; set; } = "i_";
        public string UlongKeyPrefix { get; set; } = "i_";
        public string DecimalKeyPrefix { get; set; } = "d_";
        public string ObjectKeyPrefix { get; set; } = "o_";

        public JsonConverter<DateTimeOffset> DateTimeOffsetConverter { get; set; } = new DefaultDateTimeOffsetConverter();
        public JsonConverter<DateTime> DateTimeConverter { get; set; } = new DefaultDateTimeConverter();
        public JsonConverter<Enum> EnumConverter { get; set; } = new DefaultEnumConverter();
    }
}