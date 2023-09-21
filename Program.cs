using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

//Aquí especificamos el servidor Rabbit MQ. Usamos la imagen docker de Rabbitmq y la usamos.
var factory = new ConnectionFactory
{
    HostName = "localhost"
};

//Crea la conexión RabbitMQ usando los detalles de la fábrica de conexiones
using var connection = factory.CreateConnection();

//Aquí creamos canal con sesión y modelo
using var channel = connection.CreateModel();

//declarar la cola después de mencionar el nombre y algunas propiedades relacionadas
channel.QueueDeclare("product", exclusive: false);

Console.WriteLine(" [*] Waiting for messages.");

//Establece el objeto de evento que escucha el mensaje del canal enviado por el productor
var consumer = new EventingBasicConsumer(channel);

consumer.Received += (model, eventArgs) => {
    var body = eventArgs.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($" [x] Received {message}");

    //------------------------------------
    dynamic obj = JObject.Parse(message);
    Console.WriteLine((string)obj.ProductId);
    Console.WriteLine((string)obj.ProductName);
    //------------------------------------

    Console.WriteLine($"Product message received: {message}");
};
//lee el mensaje
channel.BasicConsume(queue: "product", autoAck: true, consumer: consumer);
Console.ReadKey();