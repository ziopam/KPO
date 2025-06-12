using KR3.OrderService.Models;
using KR3.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace KR3.OrderService.Tests
{
    public class OrderServiceTests
    {
        private readonly OrderDbContext _dbContext;
        private readonly TestOrderService _orderService;
        private readonly ILogger<TestOrderService> _logger;

        public OrderServiceTests()
        {
            var options = new DbContextOptionsBuilder<OrderDbContext>()
                .UseInMemoryDatabase(databaseName: $"OrderTestDb_{Guid.NewGuid()}")
                .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            _dbContext = new OrderDbContext(options);
            _logger = NullLogger<TestOrderService>.Instance;
            _orderService = new TestOrderService(_dbContext, _logger);
        }
        [Fact]
        public async Task CreateOrder_ShouldCreateOrderAndOutboxMessage()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var request = new CreateOrderRequest
            {
                UserId = userId,
                Amount = 100.00m,
                Description = "Test Order"
            };

            // Act
            var result = await _orderService.CreateOrderAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.UserId);
            Assert.Equal(100.00m, result.Amount);
            Assert.Equal("Test Order", result.Description);
            Assert.Equal(OrderStatus.PaymentPending, result.Status);
            var savedOrder = await _dbContext.Orders.FindAsync(result.OrderId);
            Assert.NotNull(savedOrder);
            Assert.Equal(userId, savedOrder.UserId);
            Assert.Equal(OrderStatus.PaymentPending, savedOrder.Status);
        }

        [Fact]
        public async Task GetOrderById_ShouldReturnOrder_WhenOrderExists()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var order = new Order
            {
                Id = orderId,
                UserId = userId,
                Amount = 250.00m,
                Description = "Existing Order",
                Status = OrderStatus.Created,
                CreatedAt = DateTime.UtcNow
            };

            await _dbContext.Orders.AddAsync(order);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _orderService.GetOrderByIdAsync(orderId, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(orderId, result.OrderId);
            Assert.Equal(userId, result.UserId);
            Assert.Equal(250.00m, result.Amount);
        }

        [Fact]
        public async Task GetUserOrders_ShouldReturnUserOrders()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var order1 = new Order
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Amount = 100.00m,
                Description = "Order 1",
                Status = OrderStatus.Created,
                CreatedAt = DateTime.UtcNow
            };

            var order2 = new Order
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Amount = 200.00m,
                Description = "Order 2",
                Status = OrderStatus.PaymentPending,
                CreatedAt = DateTime.UtcNow.AddMinutes(5)
            };

            await _dbContext.Orders.AddRangeAsync(order1, order2);
            await _dbContext.SaveChangesAsync();

            // Act
            var results = await _orderService.GetOrdersForUserAsync(userId);

            // Assert
            Assert.NotNull(results);
            Assert.Equal(2, results.Count);
            Assert.Contains(results, o => o.Description == "Order 1");
            Assert.Contains(results, o => o.Description == "Order 2");
        }

        [Fact]
        public async Task UpdateOrderStatus_ShouldUpdateStatus()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var order = new Order
            {
                Id = orderId,
                UserId = Guid.NewGuid(),
                Amount = 300.00m,
                Description = "Order to update",
                Status = OrderStatus.PaymentPending,
                CreatedAt = DateTime.UtcNow
            };

            await _dbContext.Orders.AddAsync(order);
            await _dbContext.SaveChangesAsync();

            // Act
            await _orderService.UpdateOrderStatusAsync(orderId, OrderStatus.Completed);

            // Assert
            var updatedOrder = await _dbContext.Orders.FindAsync(orderId);
            Assert.NotNull(updatedOrder);
            Assert.Equal(OrderStatus.Completed, updatedOrder.Status);
        }
    }
}
