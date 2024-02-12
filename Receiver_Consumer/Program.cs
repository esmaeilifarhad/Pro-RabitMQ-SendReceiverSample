
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


        var headers = new Dictionary<string, object> {
            {"subject","order" },
             {"action","create" },
            {"x-match","any" }
        };


        channel.QueueDeclare(
            queue: "orderService"
            , durable: false
            ,exclusive: false
            ,autoDelete: false);

        channel.QueueBind(
            queue: "orderService"
            , exchange: "orderHeader"
            , routingKey: ""
            ,arguments:headers);

        var consumer = new EventingBasicConsumer(channel);
        //دریافت پیام
        consumer.Received += (model, eventArg) =>
                {
                    var body = eventArg.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine("eventArg :"+eventArg.DeliveryTag  +" Received Message " + message);
                  
                };


        //عکس العمل به فرستنده پیام
        channel.BasicConsume(
            queue: "orderService", 
            autoAck: true, 
            consumer: consumer);
        Console.ReadLine();

    }
}
