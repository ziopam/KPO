using KR3.PaymentService.Models;
using KR3.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace KR3.PaymentService.Services
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
                Password = _messageBusSettings.Password,
                RequestedConnectionTimeout = _messageBusSettings.RequestedConnectionTimeout,
                SocketReadTimeout = _messageBusSettings.SocketReadTimeout,
                SocketWriteTimeout = _messageBusSettings.SocketWriteTimeout,
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
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

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (_connection == null || !_connection.IsOpen || _channel == null || !_channel.IsOpen)
                    {
                        try
                        {
                            _logger.LogInformation("Attempting to connect to RabbitMQ...");

                            if (_channel != null)
                            {
                                _channel.Dispose();
                                _channel = null;
                            }

                            if (_connection != null)
                            {
                                _connection.Dispose();
                                _connection = null;
                            }

                            _connection = _connectionFactory.CreateConnection();
                            _channel = _connection.CreateModel();

                            _channel.QueueDeclare(
                                queue: _messageBusSettings.OrderPaymentResponseQueue,
                                durable: true,
                                exclusive: false,
                                autoDelete: false,
                                arguments: null);

                            _logger.LogInformation("Successfully connected to RabbitMQ");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Failed to connect to RabbitMQ. Will retry in 5 seconds.");
                            await Task.Delay(5000, stoppingToken);
                            continue;
                        }
                    }

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
            var dbContext = scope.ServiceProvider.GetRequiredService<PaymentDbContext>();

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
                    message.Error = null;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing outbox message {MessageId}", message.Id);
                    message.Error = ex.Message;
                }
            }

            await dbContext.SaveChangesAsync(cancellationToken);
        }

        private Task ProcessMessageAsync(OutboxMessage message, PaymentDbContext dbContext, CancellationToken cancellationToken)
        {
            switch (message.Type)
            {
                case "PaymentResponse":
                    var paymentResponse = JsonSerializer.Deserialize<PaymentResponse>(message.Data);
                    SendPaymentResponse(paymentResponse);
                    _logger.LogInformation("Sent payment response for order {OrderId}", paymentResponse?.OrderId);
                    break;
                default:
                    _logger.LogWarning("Unknown message type: {MessageType}", message.Type);
                    break;
            }

            return Task.CompletedTask;
        }

        private void SendPaymentResponse(PaymentResponse paymentResponse)
        {
            try
            {
                if (_channel != null && _channel.IsOpen)
                {
                    var messageBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(paymentResponse));
                    _channel.BasicPublish(
                        exchange: "",
                        routingKey: _messageBusSettings.OrderPaymentResponseQueue,
                        basicProperties: null,
                        body: messageBytes);
                }
                else
                {
                    _logger.LogWarning("Cannot send payment response: RabbitMQ channel is not ready");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send payment response to RabbitMQ");
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
