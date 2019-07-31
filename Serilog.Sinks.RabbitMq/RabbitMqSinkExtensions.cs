using System;
using RabbitMQ.Client;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;

namespace Serilog.Sinks.RabbitMq
{
    public static class RabbitMqSinkExtensions
    {
        public static LoggerSinkConfiguration RabbitMq(
            this LoggerSinkConfiguration configuration, 
            RabbitMqSinkOptions options,
            AmqpTcpEndpoint endpoint,
            Action<ConnectionFactory> connectionFactorySetup,
            ITextFormatter formatter,
            LogEventLevel restrictedToMinimumLevel = LogEventLevel.Verbose,
            LoggingLevelSwitch levelSwitch = null,
            TextToBinaryFormatterOptions formatterOptions = null,
            string clientProviderName = null)
        {
            RabbitMqSink rabbitMqSink = new RabbitMqSink(options, endpoint, connectionFactorySetup, formatter,
                formatterOptions, clientProviderName);

            configuration.Sink(rabbitMqSink, restrictedToMinimumLevel, levelSwitch);

            return configuration;
        }
    }
}