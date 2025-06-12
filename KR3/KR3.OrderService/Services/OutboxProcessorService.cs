using KR3.OrderService.Models;
using KR3.Shared.Models;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace KR3.OrderService.Services
{
    public class OutboxProcessorService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<OutboxProcessorService> _logger;
        private readonly MessageBusSettings _messageBusSettings;
        private readonly ConnectionFactory _connectionFactory;
        private readonly TimeSpan _processingInterval = TimeSpan.FromSeconds(5);
        private IConnection _connection;
        private IModel _channel;

        public OutboxProcessorService(
            IServiceProvider serviceProvider,
            ILogger<OutboxProcessorService> logger,
            MessageBusSettings messageBusSettings)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _messageBusSettings = messageBusSettings;

            _connectionFactory = new ConnectionFactory
            {
                HostName = _messageBusSettings.Host,
                Port = _messageBusSettings.Port,
                UserName = _messageBusSettings.UserName,
                Password = _messageBusSettings.Password
            };
        }
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Outbox processor service starting");

            return base.StartAsync(cancellationToken);
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Outbox processor service is running");

            try
            {
                _connection = _connectionFactory.CreateConnection();
                _channel = _connection.CreateModel();

                _channel.QueueDeclare(
                    queue: _messageBusSettings.OrderPaymentRequestQueue,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                _channel.QueueDeclare(
                    queue: _messageBusSettings.OrderPaymentResponseQueue,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                _logger.LogInformation("RabbitMQ connection established successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to connect to RabbitMQ. Will retry on each processing cycle.");
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessOutboxMessagesAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing outbox messages");
                }

                await Task.Delay(_processingInterval, stoppingToken);
            }
        }

        private async Task ProcessOutboxMessagesAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<OrderDbContext>();

            var messages = await dbContext.OutboxMessages
                .Where(m => m.ProcessedUtc == null)
                .OrderBy(m => m.CreatedUtc)
                .Take(20)
                .ToListAsync(cancellationToken);

            if (!messages.Any())
            {
                return;
            }

            _logger.LogInformation("Found {Count} outbox messages to process", messages.Count);

            foreach (var message in messages)
            {
                try
                {
                    await ProcessMessageAsync(message, dbContext, cancellationToken);

                    message.ProcessedUtc = DateTime.UtcNow;
                    message.Error = string.Empty;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing outbox message {MessageId}", message.Id);
                    message.Error = ex.Message;
                }
            }

            await dbContext.SaveChangesAsync(cancellationToken);
        }

        private Task ProcessMessageAsync(OutboxMessage message, OrderDbContext dbContext, CancellationToken cancellationToken)
        {
            switch (message.Type)
            {
                case "PaymentRequest":
                    var paymentRequest = JsonSerializer.Deserialize<PaymentRequest>(message.Data);
                    SendPaymentRequest(paymentRequest);
                    _logger.LogInformation("Sent payment request for order {OrderId}", paymentRequest?.OrderId);
                    break;
                default:
                    _logger.LogWarning("Unknown message type: {MessageType}", message.Type);
                    break;
            }

            return Task.CompletedTask;
        }
        private void SendPaymentRequest(PaymentRequest paymentRequest)
        {
            try
            {
                if (_connection == null || !_connection.IsOpen || _channel == null || !_channel.IsOpen)
                {
                    _logger.LogWarning("RabbitMQ connection is not available. Attempting to reconnect...");

                    try
                    {
                        _connection?.Close();
                        _channel?.Close();

                        _connection = _connectionFactory.CreateConnection();
                        _channel = _connection.CreateModel();

                        _channel.QueueDeclare(
                            queue: _messageBusSettings.OrderPaymentRequestQueue,
                            durable: true,
                            exclusive: false,
                            autoDelete: false,
                            arguments: null);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to reconnect to RabbitMQ");
                        throw;
                    }
                }

                var messageBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(paymentRequest));
                _channel.BasicPublish(
                    exchange: "",
                    routingKey: _messageBusSettings.OrderPaymentRequestQueue,
                    basicProperties: null,
                    body: messageBytes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send payment request to RabbitMQ");
                throw;
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Outbox processor service stopping");

            _channel?.Close();
            _connection?.Close();

            await base.StopAsync(cancellationToken);
        }
    }
}
