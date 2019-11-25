using System;
using System.Collections.Generic;
using System.Text;
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

        public string DateTimeOffsetKeyPrefix { get; set; }
        public string DateTimeKeyPrefix { get; set; }
        public string StringKeyPrefix { get; set; }
        public string IntKeyPrefix { get; set; }
        public string LongKeyPrefix { get; set; }
        public string FloatKeyPrefix { get; set; } 
        public string DoubleKeyPrefix { get; set; }
        public string UintKeyPrefix { get; set; }
        public string UlongKeyPrefix { get; set; }
        public string DecimalKeyPrefix { get; set; }
        public string ObjectKeyPrefix { get; set; }
        public string ArrayKeyPrefix { get; set; }

        public JsonConverter<DateTimeOffset> DateTimeOffsetConverter { get; set; } = new DefaultDateTimeOffsetConverter();
        public JsonConverter<DateTime> DateTimeConverter { get; set; } = new DefaultDateTimeConverter();
        public JsonConverter<Enum> EnumConverter { get; set; } = new DefaultEnumConverter();

        
    }

    public static class LogEventJsonConverterOptionsExtensions
    {
        public static LogEventJsonConverterOptions WithDefaultKeyPrefixes(this LogEventJsonConverterOptions options)
        {
            options.DateTimeOffsetKeyPrefix = "dto_";
            options.DateTimeKeyPrefix = "dt_";
            options.StringKeyPrefix = "s_";
            options.IntKeyPrefix = "i_";
            options.LongKeyPrefix = "i_";
            options.FloatKeyPrefix = "f_";
            options.DoubleKeyPrefix = "f_";
            options.UintKeyPrefix = "i_";
            options.UlongKeyPrefix = "i_";
            options.DecimalKeyPrefix = "d_";
            options.ObjectKeyPrefix = "o_";
            options.ArrayKeyPrefix = "a_";

            return options;
        }

        public static LogEventJsonConverterOptions WithCamelCaseNamingPolicy(this LogEventJsonConverterOptions options)
        {
            options.JsonOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

            return options;
        }
    }
}