using System;
using System.Collections.Generic;
using Microsoft.Extensions.ObjectPool;
using Moq;
using RabbitMQ.Client;
using Serilog.Formatting.Json;
using Serilog.Sinks.RabbitMq.Client;
using Xunit;

namespace Serilog.Sinks.RabbitMq.Tests
{    
    public class ObjectPoolDisposeTests
    {
        [Fact]
        public void DoTest()
        {
            Mock<IModel> model = new Mock<IModel>();

            Mock<IConnection> connection = new Mock<IConnection>();

            connection.Setup(c => c.IsOpen).Returns(true);
            connection.Setup(c => c.CreateModel()).Returns(model.Object);

            RabbitMqSink sink = new RabbitMqSink(new RabbitMqSinkOptions(), connection.Object, new JsonFormatter());

            connection.Verify(c => c.CreateModel(), Times.Once);
            // model returned to pool and disposed because IsOpen equal false
            model.Verify(m => m.Dispose(), Times.Once);
        }
    }
}