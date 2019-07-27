using System;
using RabbitMQ.Client;
using Serilog.Events;

namespace Serilog.Sinks.RabbitMq
{
    public class RabbitMqSinkOptions
    {             
        public int ChannelsPoolMaxRetained { get; set; } = 5;

        public string Exchange { get; set; }

        /// <summary>
        /// Set to true sink will wait for publish confirmation (https://www.rabbitmq.com/confirms.html#publisher-confirms) and in case of failure exception will be thrown. 
        /// </summary>
        public bool ComfirmPublish { get; set; }

        public TimeSpan ComfirmPublishTimeout { get; set; } = TimeSpan.FromSeconds(5);

        public Func<LogEvent,string> RoutingKeyFactory { get; set; }

        public IBasicProperties BasicProperties { get; set; } = null;
    }
}