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

        private string _exchangeName = DefaultWriteToExchange;
        private string _exchangeType = RabbitMQ.Client.ExchangeType.Topic;
        private int _channelsPoolMaxRetained = 5;
        private TimeSpan _messageExpiration = TimeSpan.FromDays(1);

        public int ChannelsPoolMaxRetained
        {
            get => _channelsPoolMaxRetained;
            set => _channelsPoolMaxRetained = value >= 0 ? value : throw new ArgumentException();
        }
        public string ExchangeName
        {
            get => _exchangeName;
            set => _exchangeName = !string.IsNullOrWhiteSpace(value) ? value : throw new ArgumentException();
        }
        public bool ExchangeDurable { get; set; } = true;

        public bool ExchangeAutoDelete { get; set; } = false;

        public string ExchangeType
        {
            get => _exchangeType;
            set => _exchangeType = RabbitMQ.Client.ExchangeType.All().Contains(value)
                ? value
                : throw new ArgumentException($"Allowed values {string.Join(", ", RabbitMQ.Client.ExchangeType.All())}",
                    nameof(ExchangeType));
        }

        public IDictionary<string, object> ExchangeArguments { get; set; } = null;

        public Action<IModel> ChannelSetup { get; set; } = null;

        /// <summary>
        /// Set to true sink will wait for publish confirmation (https://www.rabbitmq.com/confirms.html#publisher-confirms) and in case of failure exception will be thrown. 
        /// </summary>
        public bool ConfirmPublish { get; set; }

        public TimeSpan ConfirmPublishTimeout { get; set; } = TimeSpan.FromSeconds(5);

        public bool MessageMandatory { get; set; } = false;

        public Func<LogEvent, string> MessageRoutingKeyFactory { get; set; } = null;

        public Action<LogEvent, IBasicProperties> MessagePropertiesSetup { get; set; } = null;

        public TimeSpan MessageExpiration
        {
            get => _messageExpiration;
            set => _messageExpiration = value.TotalMilliseconds >= 0 ? value : throw new ArgumentException();
        }
    }
}