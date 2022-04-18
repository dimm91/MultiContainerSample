// See https://aka.ms/new-console-template for more information

using RabbitMQ.Client;
using System.Text;

var hostName = "";
ConnectionFactory factory = new ConnectionFactory();

#if DEBUG
// Connect from Host -> container 'conejito'
//Since I'm using a Linus distro (WSL2), I had to specify the IP of my distro to connect it, if not you can use localhost
hostName = "172.22.51.231"; //IP of 'conejito' containers
var port = 4321; //exposed 'conejito' port
factory = new ConnectionFactory() { HostName = hostName, Port = port, UserName = "guest", Password = "guest" };
#else
// Connect from container -> container `conejito`
//This can be done either with the IP and port of the container `conejito` or with the name of the container
//hostName = "172.24.0.4";  
//var port = 5672; //internal 'conejito' container port (NOT the one we use on the host machine to access it)
//port = 5672;

// Setting the name of the container `conejito`
// For this case you should NOT use the port number
hostName = "conejito"; 
factory = new ConnectionFactory() { HostName = hostName, UserName = "guest", Password = "guest" };
#endif

var retries = 0;

while (retries < 3)
{
    try
    {
        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            channel.QueueDeclare(queue: "test-queue",
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            string message = $"This message is sent at  ${DateTime.UtcNow}";
            var body = Encoding.UTF8.GetBytes(message);

            while (true)
            {
                channel.BasicPublish(exchange: "",
                                     routingKey: "hello",
                                     basicProperties: null,
                                     body: body);
                Console.WriteLine(" [x] Sent {0}", message);
                Thread.Sleep(2000);
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }

    retries++;
    Thread.Sleep(4500);
    Console.WriteLine("Will retry to reconnect to Rabbitmq");
}

Console.WriteLine("Exited");