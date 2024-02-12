
using RabbitMQ.Client;
using System.Text;
using System.Threading.Channels;
using System;
using RabbitMQ.Client.Events;

class Program
{
    static void Main(string[] args)
    {
        //ارتباط با rabbitMq
        var factory = new ConnectionFactory();
        factory.Uri = new Uri("amqp://guest:guest@localhost:5672");
        var connection = factory.CreateConnection();
        //ایجاد کانال
        var channel = connection.CreateModel();

        //ایجاد صف
        
        channel.QueueDeclare(
            queue:"myQueue01",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        channel.BasicQos(
            prefetchSize: 0
            , prefetchCount: 1
            , global: false);


        var consumer = new EventingBasicConsumer(channel);
        //دریافت پیام
        consumer.Received += (model, eventArg) =>
                {
                    var body = eventArg.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    Random random= new Random();
                    int sleep = random.Next(0,2) * 1000;
                    Console.WriteLine($"Sleep:{sleep} delivery tags{eventArg.DeliveryTag}");
                    Thread.Sleep( sleep );
                    Console.WriteLine("Received Message " + message);
                    channel.BasicAck(eventArg.DeliveryTag, true);
                };


        //عکس العمل به فرستنده پیام
        channel.BasicConsume(
            queue: "myQueue01", 
            autoAck: false, 
            consumer: consumer);
        Console.ReadLine();

    }
}
