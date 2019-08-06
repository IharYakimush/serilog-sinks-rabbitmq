# serilog-sinks-rabbitmq
Serilog RabbitMQ Sink
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