using Microsoft.Extensions.ObjectPool;
using RabbitMQ.Client;

namespace Serilog.Sinks.RabbitMq.Client
{
    public class ChannelsPoolPolicy : IPooledObjectPolicy<IModel>
    {
        public ChannelsPoolPolicy(IConnection connection, RabbitMqSinkOptions options)
        {
            Connection = connection;
            Options = options;
        }

        public IConnection Connection { get; }
        public RabbitMqSinkOptions Options { get; }

        public IModel Create()
        {
            IModel channel = this.Connection.CreateModel();
            channel.ExchangeDeclare(this.Options.ExchangeName, this.Options.ExchangeType, this.Options.ExchangeDurable,
                this.Options.ExchangeAutoDelete, this.Options.ExchangeArguments);

            this.Options?.ChannelSetup?.Invoke(channel);

            if (this.Options.ConfirmPublish)
            {
                channel.ConfirmSelect();
            }

            return channel;
        }

        public bool Return(IModel obj)
        {
            return obj.IsOpen;
        }
    }
}