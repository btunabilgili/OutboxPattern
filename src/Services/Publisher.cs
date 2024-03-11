using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace OutboxPattern.Services
{
    public interface IPublisher
    {
        void Publish<T>(T message);
    }

    public class Publisher : IPublisher
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly ILogger<Publisher> _logger;

        public Publisher(ILogger<Publisher> logger)
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                Port = 5672,
                UserName = "user",
                Password = "password"
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(queue: "user",
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            _logger = logger;
        }

        public void Publish<T>(T message)
        {
            var messageJson = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(messageJson);

            _channel.BasicPublish(exchange: "",
                                 routingKey: "user",
                                 basicProperties: null,
                                 body: body);

            _logger.LogInformation("Message Sent {message}", messageJson);
        }
    }
}
