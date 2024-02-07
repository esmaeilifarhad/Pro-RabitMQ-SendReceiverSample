
using RabbitMQ.Client;
using System.Text;
using System.Threading.Channels;
using System;
using RabbitMQ.Client.Events;

class Program
{
    static void Main(string[] args)
    {
        var factory = new ConnectionFactory();
        factory.Uri = new Uri("amqp://guest:guest@localhost:5672");
        var connection = factory.CreateConnection();
        var channel = connection.CreateModel();


        channel.QueueDeclare(
            queue:"myQueue01",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);




        var consumer = new EventingBasicConsumer(channel);

        consumer.Received += (model, eventArg) =>
                {
                    var body = eventArg.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine("Received Message " + message);
                    channel.BasicAck(eventArg.DeliveryTag, true);
                };



        channel.BasicConsume(
            queue: "myQueue01", 
            autoAck: false, 
            consumer: consumer);
        Console.ReadLine();

    }
}
