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

        channel.QueueDeclare("myQueue01", durable: true, exclusive: false, autoDelete: false, arguments: null);

        channel.QueueDeclare("myQueue02", false, false, false, null);

        for (int i = 0; i < 100; i++)
        {
            string message = $"Send shopping cart information to place an order  Time :{DateTime.Now.Ticks}";
            var body = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish("", "myQueue01", null,body);
        }
     
       
        channel.Close();
        connection.Close();

    }
}

