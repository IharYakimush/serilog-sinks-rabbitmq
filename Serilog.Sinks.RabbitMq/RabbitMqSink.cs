using System;
using System.Collections.Generic;
using Microsoft.Extensions.ObjectPool;
using RabbitMQ.Client;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;

namespace Serilog.Sinks.RabbitMq
{
    public class RabbitMqSink : ILogEventSink, IDisposable
    {
        public const string DefaultClientProviderName = nameof(RabbitMqSink);
        public const string DefaultExchange = "exc.serilog";
        public const string Unknown = "unknown";
        public const string Source = "Source";

        public RabbitMqSink(
            ITextFormatter formatter,
            RabbitMqSinkOptions options,
            IEndpointResolver endpointResolver,
            Action<ConnectionFactory> connectionFactorySetup = null,             
            string clientProviderName = null)
        {
            if (endpointResolver == null)
            {
                throw new ArgumentNullException(nameof(endpointResolver));
            }

            Formatter = formatter ?? throw new ArgumentNullException(nameof(formatter));
            Options = options ?? throw new ArgumentNullException(nameof(options));

            this.disposeConnection = true;

            ConnectionFactory factory = new ConnectionFactory();
            connectionFactorySetup?.Invoke(factory);

            Connection = factory.CreateConnection(endpointResolver, clientProviderName ?? DefaultClientProviderName);

            this.InitChannelsPool();
        }

        public RabbitMqSink(
            ITextFormatter formatter,
            RabbitMqSinkOptions options,
            IList<AmqpTcpEndpoint> endpoints,
            Action<ConnectionFactory> connectionFactorySetup = null,
            string clientProviderName = null) : this(formatter, options, new DefaultEndpointResolver(endpoints), connectionFactorySetup, clientProviderName)
        {
        }

        public RabbitMqSink(
            ITextFormatter formatter,
            RabbitMqSinkOptions options,
            AmqpTcpEndpoint endpoint,
            Action<ConnectionFactory> connectionFactorySetup = null,
            string clientProviderName = null) : this(formatter ,options, new List<AmqpTcpEndpoint> {endpoint}, connectionFactorySetup, clientProviderName)
        {
        }

        /// <summary>
        /// Create RabbitMqSink whitch will use existing connection. In this case connection will not be closed/disposed during sink disposal.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="connection"></param>
        public RabbitMqSink(ITextFormatter formatter, RabbitMqSinkOptions options, IConnection connection)
        {
            Formatter = formatter ?? throw new ArgumentNullException(nameof(formatter));
            Options = options ?? throw new ArgumentNullException(nameof(options));
            Connection = connection ?? throw new ArgumentNullException(nameof(connection));

            if (!connection.IsOpen)
            {
                throw new ArgumentException(connection.CloseReason.ToString(), nameof(connection));
            }

            this.InitChannelsPool();            
        }

        public ITextFormatter Formatter { get; }
        public RabbitMqSinkOptions Options { get; }
        public IConnection Connection { get; private set; }
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

            this.Formatter.Format(logEvent, null);

            string routingKey = this.Options.RoutingKeyFactory?.Invoke(logEvent) ??
                           $"{logEvent.Level}.{(logEvent.Properties.ContainsKey(Source) ? logEvent.Properties[Source].ToString() : Unknown)}"
                           .ToLowerInvariant();

            string exchange = this.Options.Exchange ?? DefaultExchange;

            byte[] body = new byte[0];

            //TODO: what if channel from pool can't be used ?
            var channel = this.channelsPool.Get();

            channel.BasicPublish(exchange, routingKey, false, this.Options.BasicProperties, body);

            if (this.Options.ComfirmPublish)
            {
                channel.WaitForConfirmsOrDie(this.Options.ComfirmPublishTimeout);
            }

            this.channelsPool.Return(channel);
        }        

        public void Dispose()
        {
            if (!disposed)
            {
                if (this.disposeConnection)
                {
                    this.Connection.Close();
                    this.Connection.Dispose();
                }

                this.Connection = null;
                disposed = true;
            }

            GC.SuppressFinalize(this);
        }

        private void InitChannelsPool()
        {
            this.channelsPool = new DefaultObjectPool<IModel>(new ChannelsPoolPolicy(this.Connection, this.Options),
                this.Options.ChannelsPoolMaxRetained);

            // Try to get channel to ensure that exchange can be created without issues
            var channel = this.channelsPool.Get();
            this.channelsPool.Return(channel);
        }
    }
}