using System.Text;
using RabbitMQ.Client;

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

// Testing
for (int i = 0; i < 10; i++)
{
    var message = $"{DateTime.UtcNow} - {Guid.CreateVersion7()} - {i}";
    var body = Encoding.UTF8.GetBytes(message);

    // Publishing the message.
    await channel.BasicPublishAsync(
        exchange: string.Empty,
        routingKey: "test_messages_queue",
        mandatory: true,
        basicProperties: new BasicProperties { Persistent = true }, // Persist the message in the q.
        body: body
    );

    Console.Write($"The following message was sent to the q: {message}");

    await Task.Delay(2000);
}