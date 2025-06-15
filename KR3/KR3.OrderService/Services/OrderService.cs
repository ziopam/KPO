using KR3.OrderService.Models;
using KR3.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace KR3.OrderService.Services
{
    public interface IOrderService
    {
        Task<OrderResponse> CreateOrderAsync(CreateOrderRequest request);
        Task<OrderResponse> GetOrderByIdAsync(Guid orderId, Guid userId);
        Task<List<OrderResponse>> GetOrdersForUserAsync(Guid userId);
        Task UpdateOrderStatusAsync(Guid orderId, OrderStatus status);
    }
    public class OrderService : IOrderService
    {
        private readonly OrderDbContext _context;
        private readonly ILogger<OrderService> _logger;
        private readonly bool _isTestEnvironment;

        public OrderService(OrderDbContext context, ILogger<OrderService> logger)
        {
            _context = context;
            _logger = logger;
            _isTestEnvironment = context.Database.ProviderName == "Microsoft.EntityFrameworkCore.InMemory";
        }
        public async Task<OrderResponse> CreateOrderAsync(CreateOrderRequest request)
        {
            _logger.LogInformation("Creating order for user: {UserId}", request.UserId);

            Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction? transaction = null;
            if (!_isTestEnvironment)
            {
                transaction = await _context.Database.BeginTransactionAsync();
            }

            try
            {
                var order = new Order
                {
                    Id = Guid.NewGuid(),
                    UserId = request.UserId,
                    Amount = request.Amount,
                    Description = request.Description,
                    Status = OrderStatus.Created,
                    CreatedAt = DateTime.UtcNow
                };

                await _context.Orders.AddAsync(order);

                var paymentRequest = new PaymentRequest
                {
                    OrderId = order.Id,
                    UserId = order.UserId,
                    Amount = order.Amount,
                    Description = order.Description,
                    MessageId = Guid.NewGuid(),
                    Timestamp = DateTime.UtcNow
                };

                var outboxMessage = new OutboxMessage
                {
                    Id = Guid.NewGuid(),
                    Type = "PaymentRequest",
                    Data = JsonSerializer.Serialize(paymentRequest),
                    CreatedUtc = DateTime.UtcNow
                };

                await _context.OutboxMessages.AddAsync(outboxMessage);

                order.Status = OrderStatus.PaymentPending;
                order.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                if (transaction != null)
                {
                    await transaction.CommitAsync();
                }

                _logger.LogInformation("Order {OrderId} created successfully", order.Id);
                return order.ToDto();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order");
                if (transaction != null)
                {
                    await transaction.RollbackAsync();
                }
                throw;
            }
        }

        public async Task<OrderResponse> GetOrderByIdAsync(Guid orderId, Guid userId)
        {
            _logger.LogInformation("Getting order {OrderId} for user {UserId}", orderId, userId);

            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);

            if (order == null)
            {
                _logger.LogWarning("Order {OrderId} not found for user {UserId}", orderId, userId);
                throw new KeyNotFoundException($"Order {orderId} not found");
            }

            return order.ToDto();
        }

        public async Task<List<OrderResponse>> GetOrdersForUserAsync(Guid userId)
        {
            _logger.LogInformation("Getting orders for user {UserId}", userId);

            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return orders.Select(o => o.ToDto()).ToList();
        }

        public async Task UpdateOrderStatusAsync(Guid orderId, OrderStatus status)
        {
            _logger.LogInformation("Updating order {OrderId} status to {Status}", orderId, status);

            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                _logger.LogWarning("Order {OrderId} not found for status update", orderId);
                throw new KeyNotFoundException($"Order {orderId} not found");
            }

            order.Status = status;
            order.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }
    }
}
