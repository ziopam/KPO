using KR3.PaymentService.Models;
using KR3.Shared.Models;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace KR3.PaymentService.Services
{
    public class PaymentRequestConsumerService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<PaymentRequestConsumerService> _logger;
        private readonly MessageBusSettings _messageBusSettings;
        private readonly ConnectionFactory _connectionFactory;
        private IConnection? _connection;
        private IModel? _channel;

        public PaymentRequestConsumerService(
            IServiceProvider serviceProvider,
            ILogger<PaymentRequestConsumerService> logger,
            MessageBusSettings messageBusSettings)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _messageBusSettings = messageBusSettings; _connectionFactory = new ConnectionFactory
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
            _logger.LogInformation("Payment request consumer service starting");

            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (_connection == null || _channel == null || !_connection.IsOpen)
                    {
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

                        _logger.LogInformation("Attempting to connect to RabbitMQ...");
                        _connection = _connectionFactory.CreateConnection();
                        _channel = _connection.CreateModel();

                        _channel.QueueDeclare(
                            queue: _messageBusSettings.OrderPaymentRequestQueue,
                            durable: true,
                            exclusive: false,
                            autoDelete: false,
                            arguments: null);

                        _logger.LogInformation("Successfully connected to RabbitMQ");

                        var consumer = new EventingBasicConsumer(_channel);

                        consumer.Received += async (model, ea) =>
                        {
                            var body = ea.Body.ToArray();
                            var message = Encoding.UTF8.GetString(body);
                            var paymentRequest = JsonSerializer.Deserialize<PaymentRequest>(message);

                            _logger.LogInformation("Received payment request for order {OrderId}", paymentRequest?.OrderId);

                            try
                            {
                                if (paymentRequest != null && await IsMessageProcessedAsync(paymentRequest.MessageId))
                                {
                                    _logger.LogInformation("Payment request {MessageId} already processed, skipping", paymentRequest.MessageId);
                                    _channel.BasicAck(ea.DeliveryTag, false);
                                    return;
                                }

                                if (paymentRequest != null)
                                {
                                    await SaveToInboxAsync(paymentRequest);

                                    var result = await ProcessPaymentRequestAsync(paymentRequest);

                                    _channel.BasicAck(ea.DeliveryTag, false);
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Error processing payment request for order {OrderId}", paymentRequest?.OrderId);

                                _channel.BasicNack(ea.DeliveryTag, false, true);
                            }
                        };

                        _channel.BasicQos(0, 1, false);

                        _channel.BasicConsume(
                            queue: _messageBusSettings.OrderPaymentRequestQueue,
                            autoAck: false,
                            consumer: consumer);

                        while (!stoppingToken.IsCancellationRequested && _connection.IsOpen)
                        {
                            await Task.Delay(1000, stoppingToken);
                        }
                    }
                }
                catch (Exception ex) when (!stoppingToken.IsCancellationRequested)
                {
                    _logger.LogWarning(ex, "Error connecting to RabbitMQ. Will retry in 5 seconds.");
                    await Task.Delay(5000, stoppingToken);
                }
            }

            return;
        }

        private async Task<bool> IsMessageProcessedAsync(Guid messageId)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<PaymentDbContext>();

            return await dbContext.InboxMessages
                .AnyAsync(m => m.MessageId == messageId && m.ProcessedUtc != null);
        }

        private async Task SaveToInboxAsync(PaymentRequest request)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<PaymentDbContext>();

            if (await dbContext.InboxMessages.AnyAsync(m => m.MessageId == request.MessageId))
            {
                _logger.LogInformation("Message {MessageId} already in inbox", request.MessageId);
                return;
            }

            var inboxMessage = new InboxMessage
            {
                MessageId = request.MessageId,
                Data = JsonSerializer.Serialize(request),
                CreatedUtc = DateTime.UtcNow
            };

            await dbContext.InboxMessages.AddAsync(inboxMessage);
            await dbContext.SaveChangesAsync();

            _logger.LogInformation("Saved message {MessageId} to inbox", request.MessageId);
        }

        private async Task<bool> ProcessPaymentRequestAsync(PaymentRequest request)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<PaymentDbContext>();
            var paymentService = scope.ServiceProvider.GetRequiredService<IPaymentService>();

            using var transaction = await dbContext.Database.BeginTransactionAsync();

            try
            {
                var inboxMessage = await dbContext.InboxMessages
                    .FirstOrDefaultAsync(m => m.MessageId == request.MessageId);

                if (inboxMessage == null || inboxMessage.ProcessedUtc != null)
                {
                    _logger.LogWarning("Inbox message {MessageId} not found or already processed", request.MessageId);
                    return false;
                }

                bool paymentSuccess = await paymentService.ProcessPaymentAsync(request, dbContext);

                var paymentResponse = new PaymentResponse
                {
                    OrderId = request.OrderId,
                    Success = paymentSuccess,
                    Message = paymentSuccess ? "Payment processed successfully" : "Payment failed: insufficient funds",
                    MessageId = Guid.NewGuid(),
                    OriginalMessageId = request.MessageId,
                    Timestamp = DateTime.UtcNow
                };

                var outboxMessage = new OutboxMessage
                {
                    Type = "PaymentResponse",
                    Data = JsonSerializer.Serialize(paymentResponse),
                    CreatedUtc = DateTime.UtcNow
                };

                await dbContext.OutboxMessages.AddAsync(outboxMessage);

                inboxMessage.ProcessedUtc = DateTime.UtcNow;

                await dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Payment request {MessageId} processed, result: {Success}", request.MessageId, paymentSuccess);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payment request {MessageId}", request.MessageId);
                await transaction.RollbackAsync();
                throw;
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Payment request consumer service stopping");

            _channel?.Close();
            _connection?.Close();

            return base.StopAsync(cancellationToken);
        }
    }
}
