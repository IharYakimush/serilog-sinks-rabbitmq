using System;
using System.Collections.Generic;
using RabbitMQ.Client;
using Serilog.Events;

namespace Serilog.Sinks.RabbitMq
{
    public class RabbitMqSinkOptions
    {
        public const string DefaultWriteToExchange = "exc.serilog.logs";

        public const string DefaultAuditToExchange = "exc.serilog.audit";
        
        public int ChannelsPoolMaxRetained { get; set; } = 5;

        public string ExchangeName { get; set; } = DefaultWriteToExchange;

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

        public Func<LogEvent, string> RoutingKeyFactory { get; set; } = null;

        public IBasicProperties BasicProperties { get; set; } = null;
    }
}