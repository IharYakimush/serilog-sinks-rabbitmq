using System;
using System.Collections.Generic;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Serilog.Sinks.RabbitMq.Sample
{
    public static class Subscriber
    {
        private static readonly object SyncObj = new object();

        public static void Subscribe(string exc, string queue, string routingKey)
        {
            ConnectionFactory factory = new ConnectionFactory();
            factory.UserName = "rabbitmq";
            factory.Password = "rabbitmq";

            IConnection connection = factory.CreateConnection(new List<AmqpTcpEndpoint>{ new AmqpTcpEndpoint("localhost", 5672) });

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