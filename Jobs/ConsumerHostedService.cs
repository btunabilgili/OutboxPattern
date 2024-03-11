using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using OutboxPattern.Entities;
using System.Text.Json;

namespace OutboxPattern.Jobs
{
    public interface IConsumer
    {
        void StartConsumer();
    }

    public class Consumer : IConsumer
    {
        private readonly ILogger<Consumer> _logger;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public Consumer(ILogger<Consumer> logger)
        {
            var factory = new ConnectionFactory()
            {
                HostName = "rabbitmq",
                UserName = "user",
                Password = "password"
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _logger = logger;
        }

        public void StartConsumer()
        {
            _channel.QueueDeclare(queue: "user",
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                try
                {
                    var user = JsonSerializer.Deserialize<User>(message);

                    if (user != null)
                        _logger.LogInformation("Received message for user: {Name}, {Email}", user.Name, user.Email);
                }
                catch (JsonException ex)
                {
                    _logger.LogError(ex, "Error deserializing message: {Message}", message);
                }
            };

            _channel.BasicConsume(queue: "user",
                                 autoAck: true,
                                 consumer: consumer);
        }
    }

    public class ConsumerHostedService(IServiceProvider serviceProvider, ILogger<ConsumerHostedService> logger) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(20000, stoppingToken);

            logger.LogInformation("ConsumerHostedService started");

            using var scope = serviceProvider.CreateScope();
            var consumer = scope.ServiceProvider.GetRequiredService<IConsumer>();

            consumer.StartConsumer();

            await Task.CompletedTask;
        }
    }
}
