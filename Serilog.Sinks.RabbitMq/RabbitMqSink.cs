using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

            this.Initialize();
            this.Validate();
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
        /// Create RabbitMqSink which will use existing connection.
        /// </summary>
        /// <param name="formatter"></param>
        /// <param name="options"></param>
        /// <param name="connection">Existing opened connection</param>
        /// <param name="autoCloseConnection">If true connection will be closed and disposed during sink disposal.</param>
        public RabbitMqSink(ITextFormatter formatter, RabbitMqSinkOptions options, IConnection connection, bool autoCloseConnection = true)
        {
            Formatter = formatter ?? throw new ArgumentNullException(nameof(formatter));
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

        public ITextFormatter Formatter { get; }
        public RabbitMqSinkOptions Options { get; }
        public IConnection Connection { get; private set; }
        private ObjectPool<IModel> channelsPool;
        private ObjectPool<StringBuilder> stringBuildersPool;
        private Encoding encoding;

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

            string exchange = this.Options.ExchangeName ?? DefaultExchange;

            StringBuilder sb = this.stringBuildersPool.Get();

            using (TextWriter writer = new StringWriter(sb))
            {
                this.Formatter.Format(logEvent, writer);
            }

            byte[] body = this.encoding.GetBytes(sb.ToString());

            this.stringBuildersPool.Return(sb);

            //TODO: what if channel from pool can't be used ?
            var channel = this.channelsPool.Get();

            channel.BasicPublish(exchange, routingKey, this.Options.Mandatory, this.Options.BasicProperties, body);

            if (this.Options.ConfirmPublish)
            {
                channel.WaitForConfirmsOrDie(this.Options.ConfirmPublishTimeout);
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

        private void Initialize()
        {
            this.stringBuildersPool = new DefaultObjectPool<StringBuilder>(
                new StringBuilderPooledObjectPolicy
                {
                    InitialCapacity = this.Options.StringBuilderInitialCapacity,
                    MaximumRetainedCapacity = this.Options.StringBuilderMaxRetainedCapacity
                }, this.Options.StringBuilderPoolMaxRetainedCount);

            this.channelsPool = new DefaultObjectPool<IModel>(new ChannelsPoolPolicy(this.Connection, this.Options),
                this.Options.ChannelsPoolMaxRetained);

            this.encoding = Encoding.GetEncoding(this.Options.EncodingName);
        }

        private void Validate()
        {
            // Try to get channel to ensure that exchange can be created without issues
            var channel = this.channelsPool.Get();
            this.channelsPool.Return(channel);            
        }
    }
}