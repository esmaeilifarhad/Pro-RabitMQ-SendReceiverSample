using RabbitMQ.Client;
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        var factory = new ConnectionFactory();

        factory.Uri = new Uri("amqp://guest:guest@localhost:5672");
        var connection = factory.CreateConnection();
        var channel = connection.CreateModel();


        channel.ExchangeDeclare(
            exchange: "orderHeader"
            , type: ExchangeType.Headers
            , durable: false);


        string message = $"Send shopping cart information to place an order  Time :{DateTime.Now.Ticks}";
        var body = Encoding.UTF8.GetBytes(message);

        var headers = new Dictionary<string, object> {
            {"subject","order" },
             {"action","create" }
        };
        var properties=channel.CreateBasicProperties();
        properties.Headers=headers;

        channel.BasicPublish(
            exchange: "orderHeader"
            , routingKey: ""
            , basicProperties: properties
            , body);

        channel.Close();
        connection.Close();
        Console.ReadLine();
    }
}



