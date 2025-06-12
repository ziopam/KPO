using KR3.PaymentService.Models;
using KR3.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace KR3.PaymentService.Tests
{
    public class PaymentServiceTests
    {
        private readonly PaymentDbContext _dbContext;
        private readonly Services.PaymentService _paymentService;
        private readonly ILogger<Services.PaymentService> _logger;

        public PaymentServiceTests()
        {
            var options = new DbContextOptionsBuilder<PaymentDbContext>()
                .UseInMemoryDatabase(databaseName: $"PaymentTestDb_{Guid.NewGuid()}")
                .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            _dbContext = new PaymentDbContext(options);

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            _logger = loggerFactory.CreateLogger<Services.PaymentService>();

            _paymentService = new Services.PaymentService(_dbContext, _logger);
        }

        [Fact]
        public async Task CreateAccount_ShouldCreateAccount()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var request = new CreateAccountRequest
            {
                UserId = userId
            };

            // Act
            var result = await _paymentService.CreateAccountAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.UserId);
            Assert.Equal(0, result.Balance);

            var savedAccount = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.UserId == userId);
            Assert.NotNull(savedAccount);
            Assert.Equal(userId, savedAccount.UserId);
            Assert.Equal(0, savedAccount.Balance);
        }

        [Fact]
        public async Task CreateAccount_ShouldThrowException_WhenAccountAlreadyExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var existingAccount = new Account
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Balance = 0,
                CreatedAt = DateTime.UtcNow,
                Version = 1
            };

            await _dbContext.Accounts.AddAsync(existingAccount);
            await _dbContext.SaveChangesAsync();

            var request = new CreateAccountRequest
            {
                UserId = userId
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _paymentService.CreateAccountAsync(request));
        }

        [Fact]
        public async Task GetAccount_ShouldReturnAccount_WhenAccountExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var account = new Account
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Balance = 100.00m,
                CreatedAt = DateTime.UtcNow,
                Version = 1
            };

            await _dbContext.Accounts.AddAsync(account);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _paymentService.GetAccountAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.UserId);
            Assert.Equal(100.00m, result.Balance);
        }

        [Fact]
        public async Task Deposit_ShouldIncreaseBalance()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var account = new Account
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Balance = 100.00m,
                CreatedAt = DateTime.UtcNow,
                Version = 1
            };

            await _dbContext.Accounts.AddAsync(account);
            await _dbContext.SaveChangesAsync();

            var request = new DepositRequest
            {
                UserId = userId,
                Amount = 50.00m
            };

            // Act
            var result = await _paymentService.DepositToAccountAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(150.00m, result.Balance);

            var transaction = await _dbContext.Transactions
                .FirstOrDefaultAsync(t => t.AccountId == account.Id && t.Type == TransactionType.Deposit);
            Assert.NotNull(transaction);
            Assert.Equal(50.00m, transaction.Amount);
        }

        [Fact]
        public async Task ProcessPayment_ShouldSucceed_WhenSufficientFunds()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var orderId = Guid.NewGuid();
            var account = new Account
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Balance = 200.00m,
                CreatedAt = DateTime.UtcNow,
                Version = 1
            };

            await _dbContext.Accounts.AddAsync(account);
            await _dbContext.SaveChangesAsync();

            var request = new PaymentRequest
            {
                OrderId = orderId,
                UserId = userId,
                Amount = 150.00m,
                Description = "Test payment"
            };

            // Act
            var result = await _paymentService.ProcessPaymentAsync(request);

            // Assert
            Assert.True(result);

            var updatedAccount = await _dbContext.Accounts.FindAsync(account.Id);
            Assert.NotNull(updatedAccount);
            Assert.Equal(50.00m, updatedAccount.Balance);

            var transaction = await _dbContext.Transactions
                .FirstOrDefaultAsync(t => t.RelatedOrderId == orderId);
            Assert.NotNull(transaction);
            Assert.Equal(-150.00m, transaction.Amount);
            Assert.Equal(TransactionType.Payment, transaction.Type);
        }

        [Fact]
        public async Task ProcessPayment_ShouldFail_WhenInsufficientFunds()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var orderId = Guid.NewGuid();
            var account = new Account
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Balance = 50.00m,
                CreatedAt = DateTime.UtcNow,
                Version = 1
            };

            await _dbContext.Accounts.AddAsync(account);
            await _dbContext.SaveChangesAsync();

            var request = new PaymentRequest
            {
                OrderId = orderId,
                UserId = userId,
                Amount = 100.00m,
                Description = "Test payment"
            };

            // Act
            var result = await _paymentService.ProcessPaymentAsync(request);

            // Assert
            Assert.False(result);

            var updatedAccount = await _dbContext.Accounts.FindAsync(account.Id);
            Assert.NotNull(updatedAccount);
            Assert.Equal(50.00m, updatedAccount.Balance);
        }
    }
}
