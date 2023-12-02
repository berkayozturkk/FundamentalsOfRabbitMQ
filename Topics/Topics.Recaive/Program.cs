﻿using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Topics.Recaive
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            channel.ExchangeDeclare(exchange: "topics_logs", type: ExchangeType.Topic);

            var queueName = channel.QueueDeclare().QueueName;

            var rootingKey = args.Length > 0 ? args[0] : "Anonymous.info";

            channel.QueueBind(
                queue: queueName,
                exchange: "topics_logs",
                routingKey: rootingKey);

            Console.WriteLine(" [*] Waiting for logs");

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (model, ea) =>
            {
                byte[] body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($" [x] {rootingKey} : {message} log");
            };

            channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

            Console.WriteLine(" Press [enter] to exit.");

            Console.ReadLine();
        }
    }
}
