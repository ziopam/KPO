using KR3.Shared.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace KR3.OrderService.Services
{
    public class PaymentResponseConsumerService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<PaymentResponseConsumerService> _logger;
        private readonly MessageBusSettings _messageBusSettings;
        private readonly ConnectionFactory _connectionFactory;
        private IConnection _connection;
        private IModel _channel;

        public PaymentResponseConsumerService(
            IServiceProvider serviceProvider,
            ILogger<PaymentResponseConsumerService> logger,
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
            _logger.LogInformation("Payment response consumer service starting");

            _connection = _connectionFactory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(
                queue: _messageBusSettings.OrderPaymentResponseQueue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            return base.StartAsync(cancellationToken);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var paymentResponse = JsonSerializer.Deserialize<PaymentResponse>(message);

                    _logger.LogInformation("Received payment response for order {OrderId}: {Success}",
                        paymentResponse?.OrderId, paymentResponse?.Success);

                    if (paymentResponse != null)
                    {
                        await ProcessPaymentResponseAsync(paymentResponse);

                        _channel.BasicAck(ea.DeliveryTag, false);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing payment response");

                    _channel.BasicNack(ea.DeliveryTag, false, true);
                }
            };

            _channel.BasicQos(0, 1, false);

            _channel.BasicConsume(
                queue: _messageBusSettings.OrderPaymentResponseQueue,
                autoAck: false,
                consumer: consumer);

            return Task.CompletedTask;
        }

        private async Task ProcessPaymentResponseAsync(PaymentResponse paymentResponse)
        {
            using var scope = _serviceProvider.CreateScope();
            var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();

            var newStatus = paymentResponse.Success ? OrderStatus.Completed : OrderStatus.PaymentFailed;

            await orderService.UpdateOrderStatusAsync(paymentResponse.OrderId, newStatus);

            _logger.LogInformation("Updated order {OrderId} status to {Status}", paymentResponse.OrderId, newStatus);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Payment response consumer service stopping");

            _channel?.Close();
            _connection?.Close();

            return base.StopAsync(cancellationToken);
        }
    }
}
