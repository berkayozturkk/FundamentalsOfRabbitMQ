using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace WorkQueues.Worker
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ConnectionFactory factory = new() { HostName = "localhost" };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(
                queue: "task_worker",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
             );

            channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

            Console.WriteLine(" [*] Waiting for messages.");

            EventingBasicConsumer consumer = new(channel);

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($" [*] Recived : {message}");

                int dots = message.Split(".").Length - 1;
                Thread.Sleep(dots * 1000);

                Console.WriteLine(" [x] Done");
            };

            channel.BasicConsume(queue: "task_worker", autoAck: false, consumer: consumer);

            Console.WriteLine("Press [enter] to exit.");

            Console.ReadLine();
        }
    }
}
