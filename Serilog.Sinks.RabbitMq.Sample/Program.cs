using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Serilog.Core;
using Serilog.Formatting.Json;

namespace Serilog.Sinks.RabbitMq.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            LoggerConfiguration loggerConfiguration = new LoggerConfiguration();
            loggerConfiguration.WriteTo.RabbitMq(
                new RabbitMqSinkOptions {ExchangeName = "exc.logs.sample"},
                new AmqpTcpEndpoint("localhost", 5672),
                factory =>
                {
                    factory.UserName = "rabbitmq";
                    factory.Password = "rabbitmq";
                },
                new JsonFormatter());

            Logger logger = loggerConfiguration.CreateLogger();

            Subscriber.Subscribe("exc.logs.sample", "logsQueue", "#");
            
            logger.Information("Information sample");
            logger.Warning("Warning sample");

            IServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(builder => builder.AddSerilog(logger, true));

            ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

            ILogger<Program> msLogger = serviceProvider.GetRequiredService<ILogger<Program>>();

            msLogger.LogError("Some error");
        }
    }
}
