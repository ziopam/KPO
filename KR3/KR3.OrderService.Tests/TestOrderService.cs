using KR3.OrderService.Models;
using KR3.OrderService.Services;
using KR3.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace KR3.OrderService.Tests
{
    public class TestOrderService : IOrderService
    {
        private readonly OrderDbContext _context;
        private readonly ILogger<TestOrderService> _logger;

        public TestOrderService(OrderDbContext context, ILogger<TestOrderService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<OrderResponse> CreateOrderAsync(CreateOrderRequest request)
        {
            _logger.LogInformation("Creating test order for user: {UserId}", request.UserId);

            var order = new Order
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                Amount = request.Amount,
                Description = request.Description,
                Status = OrderStatus.PaymentPending,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Test order {OrderId} created successfully", order.Id);
            return order.ToDto();
        }

        public async Task<OrderResponse> GetOrderByIdAsync(Guid orderId, Guid userId)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);

            if (order == null)
            {
                throw new KeyNotFoundException($"Order {orderId} not found");
            }

            return order.ToDto();
        }

        public async Task<List<OrderResponse>> GetOrdersForUserAsync(Guid userId)
        {
            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return orders.Select(o => o.ToDto()).ToList();
        }

        public async Task UpdateOrderStatusAsync(Guid orderId, OrderStatus status)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                throw new KeyNotFoundException($"Order {orderId} not found");
            }

            order.Status = status;
            order.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }
    }
}
