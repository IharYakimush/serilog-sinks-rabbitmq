using System.Collections.Generic;
using System.Linq;
using RabbitMQ.Client;

namespace Serilog.Sinks.RabbitMq
{
    public static class RabbitMqSinkOptionsExtensions
    {
        public static RabbitMqSinkOptions WithEndpointResolver(this RabbitMqSinkOptions options, IEndpointResolver resolver)
        {
            if (options == null)
            {
                throw new System.ArgumentNullException(nameof(options));
            }

            options.EndpointResolver = resolver ?? throw new System.ArgumentNullException(nameof(resolver));

            return options;
        }

        public static RabbitMqSinkOptions WithEndpoints(this RabbitMqSinkOptions options, IEnumerable<AmqpTcpEndpoint> endpoints)
        {
            return options.WithEndpointResolver(new DefaultEndpointResolver(endpoints));
        }

        public static RabbitMqSinkOptions WithEndpoint(this RabbitMqSinkOptions options, AmqpTcpEndpoint endpoint)
        {
            return options.WithEndpoints(Enumerable.Repeat(endpoint, 1));                        
        }
    }
}