﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog.Core;
using Serilog.Formatting.Json;

namespace Serilog.Sinks.RabbitMq.Sample
{
    class Program
    {
        private static readonly object SyncObj = new object();

        static void Main(string[] args)
        {
            LoggerConfiguration loggerConfiguration = new LoggerConfiguration();
            loggerConfiguration.WriteTo.RabbitMq(
                options => options.ExchangeName = "exc.logs.sample",
                new AmqpTcpEndpoint("localhost", 5672),
                factory =>
                {
                    factory.UserName = "rabbitmq";
                    factory.Password = "rabbitmq";
                },
                new JsonFormatter());

            Logger logger = loggerConfiguration.CreateLogger();

            Subscribe("exc.logs.sample", "logsQueue", "#");
            
            logger.Information("Information sample");
            logger.Warning("Warning sample");

            IServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(builder => builder.AddSerilog(logger, true));

            ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

            ILogger<Program> msLogger = serviceProvider.GetRequiredService<ILogger<Program>>();

            msLogger.LogError("Some error");
        }

        public static void Subscribe(string exc, string queue, string routingKey)
        {
            ConnectionFactory factory = new ConnectionFactory();
            factory.UserName = "rabbitmq";
            factory.Password = "rabbitmq";

            IConnection connection = factory.CreateConnection(new List<AmqpTcpEndpoint> { new AmqpTcpEndpoint("localhost", 5672) });

            IModel channel = connection.CreateModel();
            channel.QueueDeclare(queue, true, false, false);
            channel.QueueBind(queue, exc, routingKey);

            EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
            consumer.Received += (o, e) =>
            {
                lock (SyncObj)
                {
                    Console.WriteLine($"{e.RoutingKey}: {e.Body.Length}bytes");
                }

                // Confirm that message processed successfully
                channel.BasicAck(e.DeliveryTag, false);
            };

            channel.BasicConsume(queue, false, consumer);
        }
    }
}
