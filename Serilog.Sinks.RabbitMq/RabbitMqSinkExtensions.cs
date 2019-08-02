using System;
using System.Collections.Generic;
using RabbitMQ.Client;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;

namespace Serilog.Sinks.RabbitMq
{
    public static class RabbitMqSinkExtensions
    {
        private static RabbitMqSinkOptions Create(this LoggerSinkConfiguration configuration, Action<RabbitMqSinkOptions> optionsSetup)
        {
            if (optionsSetup == null)
            {
                throw new ArgumentNullException(nameof(optionsSetup));
            }

            RabbitMqSinkOptions options = new RabbitMqSinkOptions();
            optionsSetup.Invoke(options);

            return options;
        }

        private static RabbitMqSinkOptions Create(this LoggerAuditSinkConfiguration configuration, Action<RabbitMqSinkOptions> optionsSetup)
        {
            if (optionsSetup == null)
            {
                throw new ArgumentNullException(nameof(optionsSetup));
            }

            RabbitMqSinkOptions options = new RabbitMqSinkOptions();

            // Set confirm publish by default for Audit
            options.ConfirmPublish = true;
            options.ExchangeName = RabbitMqSinkOptions.DefaultAuditToExchange;
            optionsSetup.Invoke(options);

            return options;
        }

        public static LoggerSinkConfiguration RabbitMq(
            this LoggerSinkConfiguration configuration, 
            Action<RabbitMqSinkOptions> optionsSetup,
            AmqpTcpEndpoint endpoint,
            Action<ConnectionFactory> connectionFactorySetup,
            ITextFormatter formatter,
            LogEventLevel restrictedToMinimumLevel = LogEventLevel.Verbose,
            LoggingLevelSwitch levelSwitch = null,
            TextToBinaryFormatterOptions formatterOptions = null,
            string clientProviderName = null)
        {
            RabbitMqSinkOptions options = configuration.Create(optionsSetup);

            RabbitMqSink rabbitMqSink = new RabbitMqSink(options, endpoint, connectionFactorySetup, formatter,
                formatterOptions, clientProviderName);

            configuration.Sink(rabbitMqSink, restrictedToMinimumLevel, levelSwitch);

            return configuration;
        }

        public static LoggerAuditSinkConfiguration RabbitMq(
            this LoggerAuditSinkConfiguration configuration,
            Action<RabbitMqSinkOptions> optionsSetup,
            AmqpTcpEndpoint endpoint,
            Action<ConnectionFactory> connectionFactorySetup,
            ITextFormatter formatter,
            LogEventLevel restrictedToMinimumLevel = LogEventLevel.Verbose,
            LoggingLevelSwitch levelSwitch = null,
            TextToBinaryFormatterOptions formatterOptions = null,
            string clientProviderName = null)
        {
            RabbitMqSinkOptions options = configuration.Create(optionsSetup);

            RabbitMqSink rabbitMqSink = new RabbitMqSink(options, endpoint, connectionFactorySetup, formatter,
                formatterOptions, clientProviderName);

            configuration.Sink(rabbitMqSink, restrictedToMinimumLevel, levelSwitch);

            return configuration;
        }

        public static LoggerSinkConfiguration RabbitMq(
            this LoggerSinkConfiguration configuration,
            Action<RabbitMqSinkOptions> optionsSetup,
            IList<AmqpTcpEndpoint> endpoints,
            Action<ConnectionFactory> connectionFactorySetup,
            ITextFormatter formatter,
            LogEventLevel restrictedToMinimumLevel = LogEventLevel.Verbose,
            LoggingLevelSwitch levelSwitch = null,
            TextToBinaryFormatterOptions formatterOptions = null,
            string clientProviderName = null)
        {
            RabbitMqSinkOptions options = configuration.Create(optionsSetup);

            RabbitMqSink rabbitMqSink = new RabbitMqSink(options, endpoints, connectionFactorySetup, formatter,
                formatterOptions, clientProviderName);

            configuration.Sink(rabbitMqSink, restrictedToMinimumLevel, levelSwitch);

            return configuration;
        }

        public static LoggerAuditSinkConfiguration RabbitMq(
            this LoggerAuditSinkConfiguration configuration,
            Action<RabbitMqSinkOptions> optionsSetup,
            IList<AmqpTcpEndpoint> endpoints,
            Action<ConnectionFactory> connectionFactorySetup,
            ITextFormatter formatter,
            LogEventLevel restrictedToMinimumLevel = LogEventLevel.Verbose,
            LoggingLevelSwitch levelSwitch = null,
            TextToBinaryFormatterOptions formatterOptions = null,
            string clientProviderName = null)
        {
            RabbitMqSinkOptions options = configuration.Create(optionsSetup);

            RabbitMqSink rabbitMqSink = new RabbitMqSink(options, endpoints, connectionFactorySetup, formatter,
                formatterOptions, clientProviderName);

            configuration.Sink(rabbitMqSink, restrictedToMinimumLevel, levelSwitch);

            return configuration;
        }

        public static LoggerSinkConfiguration RabbitMq(
            this LoggerSinkConfiguration configuration,
            Action<RabbitMqSinkOptions> optionsSetup,
            IList<AmqpTcpEndpoint> endpoints,
            Action<ConnectionFactory> connectionFactorySetup,
            IBinaryFormatter binaryFormatter,
            LogEventLevel restrictedToMinimumLevel = LogEventLevel.Verbose,
            LoggingLevelSwitch levelSwitch = null,
            string clientProviderName = null)
        {
            RabbitMqSinkOptions options = configuration.Create(optionsSetup);

            RabbitMqSink rabbitMqSink = new RabbitMqSink(options, endpoints, connectionFactorySetup, binaryFormatter,
                clientProviderName);

            configuration.Sink(rabbitMqSink, restrictedToMinimumLevel, levelSwitch);

            return configuration;
        }

        public static LoggerAuditSinkConfiguration RabbitMq(
            this LoggerAuditSinkConfiguration configuration,
            Action<RabbitMqSinkOptions> optionsSetup,
            IList<AmqpTcpEndpoint> endpoints,
            Action<ConnectionFactory> connectionFactorySetup,
            IBinaryFormatter binaryFormatter,
            LogEventLevel restrictedToMinimumLevel = LogEventLevel.Verbose,
            LoggingLevelSwitch levelSwitch = null,
            string clientProviderName = null)
        {
            RabbitMqSinkOptions options = configuration.Create(optionsSetup);

            RabbitMqSink rabbitMqSink = new RabbitMqSink(options, endpoints, connectionFactorySetup, binaryFormatter,
                clientProviderName);

            configuration.Sink(rabbitMqSink, restrictedToMinimumLevel, levelSwitch);

            return configuration;
        }

        public static LoggerSinkConfiguration RabbitMq(
            this LoggerSinkConfiguration configuration,
            Action<RabbitMqSinkOptions> optionsSetup,
            AmqpTcpEndpoint endpoint,
            Action<ConnectionFactory> connectionFactorySetup,
            IBinaryFormatter binaryFormatter,
            LogEventLevel restrictedToMinimumLevel = LogEventLevel.Verbose,
            LoggingLevelSwitch levelSwitch = null,
            string clientProviderName = null)
        {
            RabbitMqSinkOptions options = configuration.Create(optionsSetup);

            RabbitMqSink rabbitMqSink = new RabbitMqSink(options, endpoint, connectionFactorySetup, binaryFormatter,
                clientProviderName);

            configuration.Sink(rabbitMqSink, restrictedToMinimumLevel, levelSwitch);

            return configuration;
        }

        public static LoggerAuditSinkConfiguration RabbitMq(
            this LoggerAuditSinkConfiguration configuration,
            Action<RabbitMqSinkOptions> optionsSetup,
            AmqpTcpEndpoint endpoint,
            Action<ConnectionFactory> connectionFactorySetup,
            IBinaryFormatter binaryFormatter,
            LogEventLevel restrictedToMinimumLevel = LogEventLevel.Verbose,
            LoggingLevelSwitch levelSwitch = null,
            string clientProviderName = null)
        {
            RabbitMqSinkOptions options = configuration.Create(optionsSetup);

            RabbitMqSink rabbitMqSink = new RabbitMqSink(options, endpoint, connectionFactorySetup, binaryFormatter,
                clientProviderName);

            configuration.Sink(rabbitMqSink, restrictedToMinimumLevel, levelSwitch);

            return configuration;
        }

        public static LoggerSinkConfiguration RabbitMq(
            this LoggerSinkConfiguration configuration,
            Action<RabbitMqSinkOptions> optionsSetup,
            IEndpointResolver endpointResolver,
            Action<ConnectionFactory> connectionFactorySetup,
            ITextFormatter formatter,
            LogEventLevel restrictedToMinimumLevel = LogEventLevel.Verbose,
            LoggingLevelSwitch levelSwitch = null,
            TextToBinaryFormatterOptions formatterOptions = null,
            string clientProviderName = null)
        {
            RabbitMqSinkOptions options = configuration.Create(optionsSetup);

            RabbitMqSink rabbitMqSink = new RabbitMqSink(options, endpointResolver, connectionFactorySetup, formatter,
                formatterOptions, clientProviderName);

            configuration.Sink(rabbitMqSink, restrictedToMinimumLevel, levelSwitch);

            return configuration;
        }

        public static LoggerAuditSinkConfiguration RabbitMq(
            this LoggerAuditSinkConfiguration configuration,
            Action<RabbitMqSinkOptions> optionsSetup,
            IEndpointResolver endpointResolver,
            Action<ConnectionFactory> connectionFactorySetup,
            ITextFormatter formatter,
            LogEventLevel restrictedToMinimumLevel = LogEventLevel.Verbose,
            LoggingLevelSwitch levelSwitch = null,
            TextToBinaryFormatterOptions formatterOptions = null,
            string clientProviderName = null)
        {
            RabbitMqSinkOptions options = configuration.Create(optionsSetup);

            RabbitMqSink rabbitMqSink = new RabbitMqSink(options, endpointResolver, connectionFactorySetup, formatter,
                formatterOptions, clientProviderName);

            configuration.Sink(rabbitMqSink, restrictedToMinimumLevel, levelSwitch);

            return configuration;
        }

        public static LoggerSinkConfiguration RabbitMq(
            this LoggerSinkConfiguration configuration,
            Action<RabbitMqSinkOptions> optionsSetup,
            IEndpointResolver endpointResolver,
            Action<ConnectionFactory> connectionFactorySetup,
            IBinaryFormatter binaryFormatter,
            LogEventLevel restrictedToMinimumLevel = LogEventLevel.Verbose,
            LoggingLevelSwitch levelSwitch = null,
            string clientProviderName = null)
        {
            RabbitMqSinkOptions options = configuration.Create(optionsSetup);

            RabbitMqSink rabbitMqSink = new RabbitMqSink(options, endpointResolver, connectionFactorySetup, binaryFormatter,
                clientProviderName);

            configuration.Sink(rabbitMqSink, restrictedToMinimumLevel, levelSwitch);

            return configuration;
        }

        public static LoggerAuditSinkConfiguration RabbitMq(
            this LoggerAuditSinkConfiguration configuration,
            Action<RabbitMqSinkOptions> optionsSetup,
            IEndpointResolver endpointResolver,
            Action<ConnectionFactory> connectionFactorySetup,
            IBinaryFormatter binaryFormatter,
            LogEventLevel restrictedToMinimumLevel = LogEventLevel.Verbose,
            LoggingLevelSwitch levelSwitch = null,
            string clientProviderName = null)
        {
            RabbitMqSinkOptions options = configuration.Create(optionsSetup);

            RabbitMqSink rabbitMqSink = new RabbitMqSink(options, endpointResolver, connectionFactorySetup, binaryFormatter,
                clientProviderName);

            configuration.Sink(rabbitMqSink, restrictedToMinimumLevel, levelSwitch);

            return configuration;
        }

        public static LoggerSinkConfiguration RabbitMq(
            this LoggerSinkConfiguration configuration,
            Action<RabbitMqSinkOptions> optionsSetup,
            IConnection connection,
            ITextFormatter formatter,
            LogEventLevel restrictedToMinimumLevel = LogEventLevel.Verbose,
            LoggingLevelSwitch levelSwitch = null,
            TextToBinaryFormatterOptions formatterOptions = null,
            bool autoCloseConnection = true)
        {
            RabbitMqSinkOptions options = configuration.Create(optionsSetup);

            RabbitMqSink rabbitMqSink = new RabbitMqSink(options, connection, formatter,
                formatterOptions, autoCloseConnection);

            configuration.Sink(rabbitMqSink, restrictedToMinimumLevel, levelSwitch);

            return configuration;
        }

        public static LoggerAuditSinkConfiguration RabbitMq(
            this LoggerAuditSinkConfiguration configuration,
            Action<RabbitMqSinkOptions> optionsSetup,
            IConnection connection,
            ITextFormatter formatter,
            LogEventLevel restrictedToMinimumLevel = LogEventLevel.Verbose,
            LoggingLevelSwitch levelSwitch = null,
            TextToBinaryFormatterOptions formatterOptions = null,
            bool autoCloseConnection = true)
        {
            RabbitMqSinkOptions options = configuration.Create(optionsSetup);

            RabbitMqSink rabbitMqSink = new RabbitMqSink(options, connection, formatter,
                formatterOptions, autoCloseConnection);

            configuration.Sink(rabbitMqSink, restrictedToMinimumLevel, levelSwitch);

            return configuration;
        }

        public static LoggerSinkConfiguration RabbitMq(
            this LoggerSinkConfiguration configuration,
            Action<RabbitMqSinkOptions> optionsSetup,
            IConnection connection,
            IBinaryFormatter binaryFormatter,
            LogEventLevel restrictedToMinimumLevel = LogEventLevel.Verbose,
            LoggingLevelSwitch levelSwitch = null,
            bool autoCloseConnection = true)
        {
            RabbitMqSinkOptions options = configuration.Create(optionsSetup);

            RabbitMqSink rabbitMqSink = new RabbitMqSink(options, connection, binaryFormatter,
                autoCloseConnection);

            configuration.Sink(rabbitMqSink, restrictedToMinimumLevel, levelSwitch);

            return configuration;
        }

        public static LoggerAuditSinkConfiguration RabbitMq(
            this LoggerAuditSinkConfiguration configuration,
            Action<RabbitMqSinkOptions> optionsSetup,
            IConnection connection,
            IBinaryFormatter binaryFormatter,
            LogEventLevel restrictedToMinimumLevel = LogEventLevel.Verbose,
            LoggingLevelSwitch levelSwitch = null,
            bool autoCloseConnection = true)
        {
            RabbitMqSinkOptions options = configuration.Create(optionsSetup);

            RabbitMqSink rabbitMqSink = new RabbitMqSink(options, connection, binaryFormatter,
                autoCloseConnection);

            configuration.Sink(rabbitMqSink, restrictedToMinimumLevel, levelSwitch);

            return configuration;
        }
    }
}