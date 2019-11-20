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
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter(null, false) }
        };

        public bool WriteException { get; set; } = true;
        public bool WriteMessage { get; set; } = true;
        public bool RenderMessage { get; set; } = true;

        public JsonConverter<DateTimeOffset> DateTimeOffsetConverter { get; set; } = new DefaultDateTimeOffsetConverter();
        public JsonConverter<Enum> EnumConverter { get; set; } = new DefaultEnumConverter();
    }
}