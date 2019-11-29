# serilog-sinks-rabbitmq
Serilog RabbitMQ Sink. Can be used with WriteTo and AuditTo. Support publish comfirmations. Use 1 connection per application and 1 channel per thread according to RabbitMQ recomendations. Has high performance JsonToUtf8 serializer
# Samples

### WriteTo with default JsonToUtf8 serializer settings
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
        });
```
Sample message:
```
{"timestamp":"2019-11-26T15:42:04.331985+03:00","level":"Error","message":"Some error 123 11/26/2019 12:42:04 { a: \u0022qwe\u0022, b: 112 } 34.7 34.7","P1":123,"p2":"2019-11-26T12:42:04.2639548Z","p3":{"a":"qwe","b":112},"p4":34.7,"p5":34.7,"EventId":{"Id":500,"Name":"UnhandledException"},"SourceContext":"Serilog.Sinks.RabbitMq.Sample.Program","exception":"System.ArgumentNullException: Value cannot be null. (Parameter \u0027msg\u0027)"}
```
### AuditTo JsonToUtf8 serializer with custom settings
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
        new LogEventJsonConverterOptions().WithCamelCaseNamingPolicy().WithWriteIndented());
```
Sample message:
```
{
  "timestamp": "2019-11-26T15:48:23.8502138+03:00",
  "level": "Error",
  "message": "Some error 123 11/26/2019 12:48:23 { a: \u0022qwe\u0022, b: 112 } 34.7 34.7",
  "p1": 123,
  "p2": "2019-11-26T12:48:23.8147876Z",
  "p3": {
    "a": "qwe",
    "b": 112
  },
  "p4": 34.7,
  "p5": 34.7,
  "eventId": {
    "id": 500,
    "name": "UnhandledException"
  },
  "sourceContext": "Serilog.Sinks.RabbitMq.Sample.Program",
  "exception": "System.ArgumentNullException: Value cannot be null. (Parameter \u0027msg\u0027)"
}
```
### WriteTo with key prefixes depended on property type (usefull for logstash/elasticsearch), camel case, indent
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
        new LogEventJsonConverterOptions().WithCamelCaseNamingPolicy().WithWriteIndented().WithDefaultKeyPrefixes());
```
Sample message:
```
{
  "timestamp": "2019-11-26T15:52:56.2795278+03:00",
  "level": "Error",
  "message": "Some error 123 11/26/2019 12:52:56 { a: \u0022qwe\u0022, b: 112 } 34.7 34.7",
  "i_p1": 123,
  "dt_p2": "2019-11-26T12:52:56.2535731Z",
  "o_p3": {
    "s_a": "qwe",
    "i_b": 112
  },
  "d_p4": 34.7,
  "f_p5": 34.7,
  "o_eventId": {
    "i_id": 500,
    "s_name": "UnhandledException"
  },
  "s_sourceContext": "Serilog.Sinks.RabbitMq.Sample.Program",
  "exception": "System.ArgumentNullException: Value cannot be null. (Parameter \u0027msg\u0027)"
}
```
### WriteTo with standard Serilog JsonFormatter
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
Sample message:
```
{"Timestamp":"2019-11-26T15:55:30.9763095+03:00","Level":"Error","MessageTemplate":"Some error {P1} {p2} {@p3} {p4} {p5}","Exception":"System.ArgumentNullException: Value cannot be null. (Parameter 'msg')","Properties":{"P1":123,"p2":"2019-11-26T12:55:30.8942973Z","p3":{"a":"qwe","b":112},"p4":34.7,"p5":34.7,"EventId":{"Id":500,"Name":"UnhandledException"},"SourceContext":"Serilog.Sinks.RabbitMq.Sample.Program"}}
```
# Nuget
https://www.nuget.org/packages/Serilog.Sinks.RabbitMq.Client
