using Microsoft.Extensions.ObjectPool;
using RabbitMQ.Client;

namespace Serilog.Sinks.RabbitMq
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
            //channel.ExchangeDeclare(this.Options.Exchange);
            if (this.Options.ComfirmPublish)
            {
                channel.ConfirmSelect();
            }

            //TODO: bind model
            return channel;
        }

        public bool Return(IModel obj)
        {
            return obj.IsOpen;
        }
    }
}