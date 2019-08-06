# serilog-sinks-rabbitmq
Serilog RabbitMQ Sink
# Samples
```
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
```
