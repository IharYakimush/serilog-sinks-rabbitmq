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
            IModel model = this.Connection.CreateModel();
            //TODO: bind model
            return model;
        }

        public bool Return(IModel obj)
        {
            return obj.IsOpen;
        }
    }
}