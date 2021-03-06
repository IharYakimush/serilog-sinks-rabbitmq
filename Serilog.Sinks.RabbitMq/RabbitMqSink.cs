﻿using System;
using System.Collections.Generic;
using Microsoft.Extensions.ObjectPool;
using RabbitMQ.Client;
using RabbitMQ.Client.Impl;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;

namespace Serilog.Sinks.RabbitMq.Client
{
    public class RabbitMqSink : ILogEventSink, IDisposable
    {
        public const string DefaultClientProviderName = nameof(RabbitMqSink);
        
        public const string Unknown = "unknown";
        public const string SourceContext = "SourceContext";

        public RabbitMqSink(
            RabbitMqSinkOptions options,
            IEndpointResolver endpointResolver,
            Action<ConnectionFactory> connectionFactorySetup,
            ITextFormatter formatter,
            TextToBinaryFormatterOptions formatterOptions = null,
            string clientProviderName = null) : this(options, endpointResolver, connectionFactorySetup,
            new TextToBinaryFormatter(formatter ?? throw new ArgumentNullException(nameof(formatter)),
                formatterOptions), clientProviderName)
        {
        }

        public RabbitMqSink(
            RabbitMqSinkOptions options,
            IEndpointResolver endpointResolver,
            Action<ConnectionFactory> connectionFactorySetup,
            IBinaryFormatter binaryFormatter,
            string clientProviderName = null)
        {
            if (endpointResolver == null)
            {
                throw new ArgumentNullException(nameof(endpointResolver));
            }

            if (connectionFactorySetup == null) throw new ArgumentNullException(nameof(connectionFactorySetup));

            Formatter = binaryFormatter ?? throw new ArgumentNullException(nameof(binaryFormatter));
            Options = options ?? throw new ArgumentNullException(nameof(options));

            this.disposeConnection = true;

            ConnectionFactory factory = new ConnectionFactory();
            connectionFactorySetup.Invoke(factory);

            Connection = factory.CreateConnection(endpointResolver, clientProviderName ?? DefaultClientProviderName);

            this.Initialize();
            this.Validate();
        }

        public RabbitMqSink(
            RabbitMqSinkOptions options,
            IList<AmqpTcpEndpoint> endpoints,
            Action<ConnectionFactory> connectionFactorySetup,
            IBinaryFormatter binaryFormatter,
            string clientProviderName = null) : this(options, new DefaultEndpointResolver(endpoints),
            connectionFactorySetup, binaryFormatter, clientProviderName)
        {
        }

        public RabbitMqSink(
            RabbitMqSinkOptions options,
            AmqpTcpEndpoint endpoint,
            Action<ConnectionFactory> connectionFactorySetup,
            IBinaryFormatter binaryFormatter,
            string clientProviderName = null) : this(options, new List<AmqpTcpEndpoint> {endpoint},
            connectionFactorySetup, binaryFormatter, clientProviderName)
        {
        }

        public RabbitMqSink(
            RabbitMqSinkOptions options,
            IList<AmqpTcpEndpoint> endpoints,
            Action<ConnectionFactory> connectionFactorySetup,
            ITextFormatter formatter,
            TextToBinaryFormatterOptions formatterOptions = null,
            string clientProviderName = null) : this(options, new DefaultEndpointResolver(endpoints),
            connectionFactorySetup, formatter, formatterOptions, clientProviderName)
        {
        }

        public RabbitMqSink(
            RabbitMqSinkOptions options,
            AmqpTcpEndpoint endpoint,
            Action<ConnectionFactory> connectionFactorySetup,
            ITextFormatter formatter,
            TextToBinaryFormatterOptions formatterOptions = null,
            string clientProviderName = null) : this(options, new List<AmqpTcpEndpoint> { endpoint },
            connectionFactorySetup, formatter, formatterOptions, clientProviderName)
        {
        }

        public RabbitMqSink(RabbitMqSinkOptions options, IConnection connection, IBinaryFormatter binaryFormatter, bool autoCloseConnection = true)
        {
            Formatter = binaryFormatter ?? throw new ArgumentNullException(nameof(binaryFormatter));
            Options = options ?? throw new ArgumentNullException(nameof(options));
            Connection = connection ?? throw new ArgumentNullException(nameof(connection));

            this.disposeConnection = autoCloseConnection;

            if (!connection.IsOpen)
            {
                throw new ArgumentException(connection.CloseReason.ToString(), nameof(connection));
            }

            this.Initialize();
            this.Validate();
        }

        /// <summary>
        /// Create RabbitMqSink which will use existing connection.
        /// </summary>
        /// <param name="formatter"></param>
        /// <param name="options"></param>
        /// <param name="connection">Existing opened connection</param>
        /// <param name="formatterOptions">Options for converting text to bytes array send in message</param>
        /// <param name="autoCloseConnection">If true connection will be closed and disposed during sink disposal.</param>
        public RabbitMqSink(RabbitMqSinkOptions options, IConnection connection, ITextFormatter formatter,
            TextToBinaryFormatterOptions formatterOptions = null,
            bool autoCloseConnection = true) : this(options, connection,
            new TextToBinaryFormatter(formatter, formatterOptions),
            autoCloseConnection)
        {
        }

        public IBinaryFormatter Formatter { get; }
        public RabbitMqSinkOptions Options { get; }
        public IConnection Connection { get; }
        private ObjectPool<IModel> channelsPool;

        private readonly bool disposeConnection = false;
        private bool disposed = false;

        public void Emit(LogEvent logEvent)
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(nameof(RabbitMqSink));
            }

            if (!this.Connection.IsOpen)
            {
                throw new InvalidOperationException(this.Connection.CloseReason.ToString());
            }

            byte[] body = this.Formatter.GetBytes(logEvent);

            string routingKey = this.Options.MessageRoutingKeyFactory?.Invoke(logEvent) ??
                           $"l.{logEvent.Level}.s.{(logEvent.Properties.ContainsKey(SourceContext) ? logEvent.Properties[SourceContext].ToString().Trim('"') : Unknown)}"
                           .ToLowerInvariant();

            IModel channel = this.channelsPool.Get();

            try
            {
                BasicProperties basicProperties = new RabbitMQ.Client.Framing.BasicProperties();
                basicProperties.Persistent = this.Options.ExchangeDurable;
                basicProperties.Expiration = ((long) this.Options.MessageExpiration.TotalMilliseconds).ToString("D");

                this.Options.MessagePropertiesSetup?.Invoke(logEvent, basicProperties);
                
                channel.BasicPublish(this.Options.ExchangeName, routingKey, this.Options.MessageMandatory, basicProperties, body);

                if (this.Options.ConfirmPublish)
                {
                    channel.WaitForConfirmsOrDie(this.Options.ConfirmPublishTimeout);
                }
            }
            finally 
            {
                this.channelsPool.Return(channel);
            }                       
        }        

        public void Dispose()
        {
            if (!disposed)
            {
                // Dispose channels opened by sink
                if (this.channelsPool is IDisposable disposable)
                {
                    disposable.Dispose();
                }

                if (this.disposeConnection)
                {
                    this.Connection.Close();
                    this.Connection.Dispose();
                }
                
                disposed = true;
            }

            GC.SuppressFinalize(this);
        }

        private void Initialize()
        {
            this.channelsPool = new DefaultObjectPoolProvider() { MaximumRetained = this.Options.ChannelsPoolMaxRetained }.Create(new ChannelsPoolPolicy(this.Connection, this.Options));
        }

        private void Validate()
        {
            // Try to get channel to ensure that exchange can be created without issues
            var channel = this.channelsPool.Get();
            this.channelsPool.Return(channel);
        }
    }
}