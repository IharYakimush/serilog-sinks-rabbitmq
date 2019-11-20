# serilog-sinks-rabbitmq
Serilog RabbitMQ Sink. Can be used with WriteTo and AuditTo. Support publish comfirmations. Use 1 connection per application and 1 channel per thread according to RabbitMQ recomendations.
# Samples

### WriteTo
```
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
        new JsonFormatter());
```
### AuditTo
```
LoggerConfiguration loggerConfiguration = new LoggerConfiguration();
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
        new JsonFormatter());
```
# Nuget
https://www.nuget.org/packages/Serilog.Sinks.RabbitMq.Client
