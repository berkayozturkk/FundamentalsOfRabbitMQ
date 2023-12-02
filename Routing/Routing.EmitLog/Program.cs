﻿using RabbitMQ.Client;
using System.Text;

namespace Routing.EmitLog
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine($"Press [enter] to send message.");
            //Console.ReadLine();

            var factory = new ConnectionFactory() { HostName = "localhost" };

            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            channel.ExchangeDeclare(exchange: "direct_logs", type: ExchangeType.Direct);

            var message = GetMessage(args);
            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(
                exchange: "direct_logs", 
                routingKey:args.Length > 0 ? args[0] :"info" ,
                basicProperties: null, 
                body: body);

            var severity = args.Length > 0 ? args[0] : "info";

            Console.WriteLine($" [x] send {severity} : {message}");
            Console.WriteLine($"Press [enter] to exit.");

            Console.ReadLine();
        }

        private static string GetMessage(string[] args)
        {
            return ((args.Length > 1) ? string.Join(", ", args.Skip(1).ToArray()) : "Hello World");
        }
    }
}
