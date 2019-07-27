using System;
using System.Collections.Generic;
using RabbitMQ.Client;
using Serilog.Events;

namespace Serilog.Sinks.RabbitMq
{
    public class RabbitMqSinkOptions
    {             
        public int ChannelsPoolMaxRetained { get; set; } = 5;

        public int StringBuilderPoolMaxRetainedCount { get; set; } = 10;

        public int StringBuilderInitialCapacity { get; set; } = 100;

        public int StringBuilderMaxRetainedCapacity { get; set; } = 5 * 1024;

        public string EncodingName { get; set; } = "UTF8";

        public string ExchangeName { get; set; } = RabbitMqSink.DefaultExchange;

        public bool ExchangeDurable { get; set; } = true;

        public bool ExchangeAutoDelete { get; set; } = false;

        public string ExchangeType { get; set; } = RabbitMQ.Client.ExchangeType.Topic;

        public IDictionary<string, object> ExchangeArguments { get; set; } = null;

        public Action<IModel> ChannelSetup { get; set; } = null;

        public bool Mandatory { get; set; } = false;

        /// <summary>
        /// Set to true sink will wait for publish confirmation (https://www.rabbitmq.com/confirms.html#publisher-confirms) and in case of failure exception will be thrown. 
        /// </summary>
        public bool ConfirmPublish { get; set; }

        public TimeSpan ConfirmPublishTimeout { get; set; } = TimeSpan.FromSeconds(5);

        public Func<LogEvent,string> RoutingKeyFactory { get; set; }

        public IBasicProperties BasicProperties { get; set; } = null;
    }
}