using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace RabbitMQ.Receive
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ConnectionFactory factory = new() { HostName = "localhost"};

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(
                queue: "hello",
                durable: false,
                exclusive:false,
                autoDelete:false,
                arguments:null
                );

            Console.WriteLine(" [*] Waiting for messages.");

            EventingBasicConsumer consumer = new(channel);

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($" [*] Recived : {message}");
            };

            channel.BasicConsume(queue:"hello",autoAck:true,consumer:consumer);

            Console.WriteLine("Press [enter] to exit.");

            Console.ReadLine();
        }
    }
}
