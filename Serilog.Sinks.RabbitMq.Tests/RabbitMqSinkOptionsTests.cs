using Serilog.Sinks.RabbitMq.Client;
using System;
using Xunit;

namespace Serilog.Sinks.RabbitMq.Tests
{
    public class RabbitMqSinkOptionsTests
    {
        [Fact]
        public void DoTest()
        {
            RabbitMqSinkOptions options = new RabbitMqSinkOptions();

            // Assert default options
            Assert.Equal("topic", options.ExchangeType);
            Assert.Throws<ArgumentException>(() => options.ExchangeType = null);
            Assert.Throws<ArgumentException>(() => options.ExchangeType = "any");
            options.ExchangeType = "fanout";


            Assert.Equal("exc.serilog.logs", options.ExchangeName);
            Assert.Throws<ArgumentException>(() => options.ExchangeName = null);
            Assert.Throws<ArgumentException>(() => options.ExchangeName = "");
            Assert.Throws<ArgumentException>(() => options.ExchangeName = " ");
            options.ExchangeName = "test";

            Assert.Null(options.MessageRoutingKeyFactory);
            Assert.Null(options.ChannelSetup);
            Assert.Null(options.MessagePropertiesSetup);

            Assert.Equal(5, options.ChannelsPoolMaxRetained);
            Assert.Throws<ArgumentException>(() => options.ChannelsPoolMaxRetained = - 1);
            options.ChannelsPoolMaxRetained = 0;

            Assert.False(options.ConfirmPublish);
            Assert.False(options.ExchangeAutoDelete);
            Assert.False(options.MessageMandatory);
            Assert.True(options.ExchangeDurable);

            Assert.Equal(5, options.ConfirmPublishTimeout.TotalSeconds);
            Assert.Equal(1, options.MessageExpiration.TotalDays);
        }
    }
}