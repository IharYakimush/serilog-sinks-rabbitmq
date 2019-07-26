using System;
using Microsoft.Extensions.ObjectPool;
using RabbitMQ.Client;
using Serilog.Core;
using Serilog.Events;

namespace Serilog.Sinks.RabbitMq
{
    public class RabbitMqSink : ILogEventSink, IDisposable
    {
        public RabbitMqSink(RabbitMqSinkOptions options, IConnection connection = null)
        {
            Options = options ?? throw new ArgumentNullException(nameof(options));

            if (connection == null)
            {
                this.disposeConnection = true;

                ConnectionFactory factory = new ConnectionFactory();
                options?.ConnectionFactorySetup?.Invoke(factory);

                bool hasName = !string.IsNullOrWhiteSpace(options.ClientProviderName);

                if (options.EndpointResolver != null && hasName)
                {
                    Connection = factory.CreateConnection(options.EndpointResolver, options.ClientProviderName);
                }
                else if (hasName)
                {
                    Connection = factory.CreateConnection(options.ClientProviderName);
                }
                else
                {
                    Connection = factory.CreateConnection();
                }
            }
            else
            {
                Connection = connection;
            }

            this.modelsPool = new DefaultObjectPool<IModel>(new ChannelsPoolPolicy(this.Connection, options),
                options.ChannelsPoolMaxRetained);
        }

        public RabbitMqSinkOptions Options { get; }
        public IConnection Connection { get; private set; }
        private readonly ObjectPool<IModel> modelsPool;

        private readonly bool disposeConnection = false;
        private bool disposedValue = false;

        public void Emit(LogEvent logEvent)
        {
            if (this.disposedValue)
            {
                throw new InvalidOperationException();
            }

            throw new System.NotImplementedException();
        }        

        public void Dispose()
        {
            if (!disposedValue)
            {
                if (this.disposeConnection)
                {
                    this.Connection.Close();
                    this.Connection.Dispose();
                }

                this.Connection = null;
                disposedValue = true;
            }

            GC.SuppressFinalize(this);
        }        
    }
}