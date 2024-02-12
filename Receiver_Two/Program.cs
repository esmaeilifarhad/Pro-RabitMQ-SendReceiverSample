using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;


class Program
{

    static void Main(string[] args)
    {
        var factory = new ConnectionFactory();
        factory.Uri = new Uri("amqp://guest:guest@localhost:5672");
        var connection = factory.CreateConnection();
        var channel = connection.CreateModel();

        var headers = new Dictionary<string, object> {
            {"subject","order" },
             {"action","create" },
            {"x-match","all" }
        };


        channel.QueueDeclare(
            queue: "emailService"
            , durable: false
            , exclusive: false
            , autoDelete: false);

        channel.QueueBind(
            queue: "emailService"
            , exchange: "orderHeader"
            , routingKey: "order.cancel"
            ,arguments:headers);


        var consumer = new EventingBasicConsumer(channel);

        consumer.Received += (model, eventArg) =>
        {
            var body = eventArg.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            var headerTitle = Encoding.UTF8.GetString(eventArg.BasicProperties.Headers["subject"] as byte[]);
            Console.WriteLine("Header : "+headerTitle +" eventArg :" + eventArg.DeliveryTag + " Received Message " + message);
        };
        channel.BasicConsume(queue: "emailService"
            , autoAck: true
            , consumer: consumer
            );
        Console.ReadLine();

    }
}




