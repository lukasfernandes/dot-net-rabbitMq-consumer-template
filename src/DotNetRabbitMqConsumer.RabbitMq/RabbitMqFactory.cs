using System.Text;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace insert_google_search_products.Base;

public class RabbitMqFactory : IRabbitMqFactory
{
    private readonly IModel _channel;
    private readonly string _exchangeName;
    private readonly string _queueName;
    private readonly string _routingKey;

    public RabbitMqFactory(IConfiguration configuration)
    {
        var rabbitMqUri = configuration["RabbitMq:Uri"];
        var queueName = configuration["RabbitMq:QueueName"];

        ArgumentException.ThrowIfNullOrWhiteSpace(rabbitMqUri);
        ArgumentException.ThrowIfNullOrWhiteSpace(queueName);

        _queueName = queueName;
        _exchangeName = $"{_queueName}_exchange";
        _routingKey = $"{_queueName}_error";

        ConnectionFactory factory = new()
        {
            Uri = new Uri(rabbitMqUri),
            ClientProvidedName = "google-search-products-consumer"
        };

        var connection = factory.CreateConnection();
        _channel = connection.CreateModel();

        _channel.ExchangeDeclare(_exchangeName, ExchangeType.Direct);
        _channel.QueueDeclare(queueName, false, false, false, null);
        _channel.QueueBind(queueName, _exchangeName, _routingKey, null);
    }

    public Task PublishMessage(string message)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(message);

        Console.WriteLine("Sending Message... ");

        var messageBodyBytes = Encoding.UTF8.GetBytes(message);
        _channel.BasicPublish(_exchangeName, _routingKey, null, messageBodyBytes);

        Console.WriteLine("Message sent");

        return Task.CompletedTask;
    }

    public Task ConsumeMessage()
    {
        _channel.BasicQos(0, 1, false);

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (sender, args) =>
        {
            var body = args.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            //
            // :TODO
            // Do anything
            //

            Console.WriteLine($"Message Received: {message}");

            _channel.BasicAck(args.DeliveryTag, false);
        };

        var consumerTag = _channel.BasicConsume(_queueName, false, consumer);

        Console.ReadLine();

        _channel.BasicCancel(consumerTag);

        return Task.CompletedTask;
    }
}