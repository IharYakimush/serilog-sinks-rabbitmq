using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Sinks.RabbitMq.Client;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog.Core;
using Serilog.Formatting.Json;
using Serilog.Sinks.RabbitMq.Json;

namespace Serilog.Sinks.RabbitMq.Sample
{
    class Program
    {
        private static readonly object SyncObj = new object();

        static void Main(string[] args)
        {
            LoggerConfiguration loggerConfiguration = new LoggerConfiguration();
            loggerConfiguration.WriteTo.RabbitMq(
                options =>
                {
                    options.ExchangeName = "exc.logs.sample";

                    // By default for WriteTo
                    options.ConfirmPublish = false;
                },
                new AmqpTcpEndpoint("localhost", 5672),
                factory =>
                {
                    factory.UserName = "rabbitmq";
                    factory.Password = "rabbitmq";
                },
                new JsonToUtf8BytesFormatter());

            LogEventJsonConverterOptions converterOptions = new LogEventJsonConverterOptions();
            converterOptions.JsonOptions.WriteIndented = true;

            loggerConfiguration.AuditTo.RabbitMq(
                options =>
                {
                    options.ExchangeName = "exc.audit.sample";

                    // By default for AuditTo
                    options.ConfirmPublish = true;
                    options.ConfirmPublishTimeout = TimeSpan.FromSeconds(5);
                },
                new AmqpTcpEndpoint("localhost", 5672),
                factory =>
                {
                    factory.UserName = "rabbitmq";
                    factory.Password = "rabbitmq";
                },
                new JsonToUtf8BytesFormatter(converterOptions));

            Logger logger = loggerConfiguration.CreateLogger();

            Subscribe("exc.logs.sample", "logsQueue", "#");
            Subscribe("exc.audit.sample", "auditQueue", "#");
            
            logger.Information("Information sample");
            logger.Warning("Warning sample");

            IServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(builder => builder.AddSerilog(logger, true));

            ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

            ILogger<Program> msLogger = serviceProvider.GetRequiredService<ILogger<Program>>();

            msLogger.LogError(new EventId(500, "UnhandledException"), new ArgumentNullException("msg"),
                "Some error {P1} {p2} {@p3} {p4} {p5}", 123, DateTime.UtcNow, new {a = "qwe", b = 112}, new decimal(34.7), 34.7f);
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
                    Console.WriteLine(Encoding.UTF8.GetString(e.Body));
                }

                // Confirm that message processed successfully
                channel.BasicAck(e.DeliveryTag, false);
            };

            channel.BasicConsume(queue, false, consumer);
        }
    }
}
