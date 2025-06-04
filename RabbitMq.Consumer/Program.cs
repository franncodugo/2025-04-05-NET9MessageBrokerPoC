using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

// Hostname of RabbitMq instance to connect. 
var factory = new ConnectionFactory { HostName = "localhost" };

// Creating a new connection to my RabbitMq broker.
using var connection = await factory.CreateConnectionAsync();

// To communicate with my broker instance.
using var channel = await connection.CreateChannelAsync();


// How I will communicate with the broker instance.
await channel.QueueDeclareAsync(
    queue: "test_messages_queue",
    durable: true, // messages will survive broker restart i.e.
    exclusive: false, // exclusive to this connection.
    autoDelete: false, // q will be deleted if last subscriber unsubscribe.
    arguments: null
);

Console.WriteLine("Waiting for messages...");

// Define the consumer by creating one.
var consumer = new AsyncEventingBasicConsumer(channel);
// Delegate asyncrono to consume the message from the Queue.
consumer.ReceivedAsync += async (sender, EventArgs) =>
{
    byte[] body = EventArgs.Body.ToArray();
    string message = Encoding.UTF8.GetString(body);

    Console.WriteLine($"Received: {message}");

    await ((AsyncEventingBasicConsumer)sender).Channel.BasicAckAsync(EventArgs.DeliveryTag, multiple: false);

};

await channel.BasicConsumeAsync("test_messages_queue", autoAck: false, consumer);

Console.ReadLine();
