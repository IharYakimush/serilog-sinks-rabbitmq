using Microsoft.Extensions.ObjectPool;
using RabbitMQ.Client;
using Serilog.Core;
using Serilog.Events;

namespace Serilog.Sinks.RabbitMq
{
    public class RabbitMqSink : ILogEventSink
    {
        public RabbitMqSink(RabbitMqSinkOptions options, IConnection connection = null)
        {
            Options = options ?? throw new System.ArgumentNullException(nameof(options));

            if (connection == null)
            {
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
        public IConnection Connection { get; }
        private readonly ObjectPool<IModel> modelsPool;

        public void Emit(LogEvent logEvent)
        {
            throw new System.NotImplementedException();
        }
    }
}