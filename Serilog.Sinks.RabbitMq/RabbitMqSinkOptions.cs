using System;
using RabbitMQ.Client;

namespace Serilog.Sinks.RabbitMq
{
    public class RabbitMqSinkOptions
    {
        public const string DefaultClientProviderName = nameof(RabbitMqSink);

        public string ClientProviderName { get; set; } = DefaultClientProviderName;

        public IEndpointResolver EndpointResolver { get; internal set; } = null;

        public Action<ConnectionFactory> ConnectionFactorySetup { get; set; } = null;

        public int ChannelsPoolMaxRetained { get; set; } = 5;
    }
}