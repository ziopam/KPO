using KR3.PaymentService.Models;
using KR3.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace KR3.PaymentService.Services
{
    public interface IPaymentService
    {
        Task<AccountResponse> CreateAccountAsync(CreateAccountRequest request);
        Task<AccountResponse> GetAccountAsync(Guid userId);
        Task<AccountResponse> DepositToAccountAsync(DepositRequest request);
        Task<bool> ProcessPaymentAsync(PaymentRequest request);
        Task<bool> ProcessPaymentAsync(PaymentRequest request, PaymentDbContext dbContext);
    }

    public class PaymentService : IPaymentService
    {
        private readonly PaymentDbContext _context;
        private readonly ILogger<PaymentService> _logger;

        public PaymentService(PaymentDbContext context, ILogger<PaymentService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<AccountResponse> CreateAccountAsync(CreateAccountRequest request)
        {
            _logger.LogInformation("Creating account for user: {UserId}", request.UserId);

            if (await _context.Accounts.AnyAsync(a => a.UserId == request.UserId))
            {
                _logger.LogWarning("Account already exists for user {UserId}", request.UserId);
                throw new InvalidOperationException($"Account already exists for user {request.UserId}");
            }

            var account = new Account
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                Balance = 0,
                CreatedAt = DateTime.UtcNow,
                Version = 1
            };

            await _context.Accounts.AddAsync(account);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Account {AccountId} created for user {UserId}", account.Id, request.UserId);
            return account.ToDto();
        }

        public async Task<AccountResponse> GetAccountAsync(Guid userId)
        {
            _logger.LogInformation("Getting account for user: {UserId}", userId);

            var account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.UserId == userId);

            if (account == null)
            {
                _logger.LogWarning("Account not found for user {UserId}", userId);
                throw new KeyNotFoundException($"Account not found for user {userId}");
            }

            return account.ToDto();
        }
        public async Task<AccountResponse> DepositToAccountAsync(DepositRequest request)
        {
            _logger.LogInformation("Depositing {Amount} to account for user {UserId}", request.Amount, request.UserId);

            if (request.Amount <= 0)
            {
                throw new ArgumentException("Deposit amount must be positive");
            }

            try
            {
                var account = await _context.Accounts
                    .FirstOrDefaultAsync(a => a.UserId == request.UserId);

                if (account == null)
                {
                    _logger.LogWarning("Account not found for user {UserId}", request.UserId);
                    throw new KeyNotFoundException($"Account not found for user {request.UserId}");
                }

                var transactionRecord = new Transaction
                {
                    Id = Guid.NewGuid(),
                    AccountId = account.Id,
                    Amount = request.Amount,
                    Description = "Deposit",
                    Type = TransactionType.Deposit,
                    CreatedAt = DateTime.UtcNow
                };

                await _context.Transactions.AddAsync(transactionRecord);

                account.Balance += request.Amount;
                account.UpdatedAt = DateTime.UtcNow;
                account.Version++;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Deposit successful for user {UserId}, new balance: {Balance}",
                    request.UserId, account.Balance);

                return account.ToDto();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Concurrency conflict during deposit for user {UserId}", request.UserId);
                throw new Exception("A concurrency conflict occurred. Please try again.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during deposit for user {UserId}", request.UserId);
                throw;
            }
        }
        public async Task<bool> ProcessPaymentAsync(PaymentRequest request)
        {
            return await ProcessPaymentAsync(request, _context);
        }

        public async Task<bool> ProcessPaymentAsync(PaymentRequest request, PaymentDbContext dbContext)
        {
            _logger.LogInformation("Processing payment for order {OrderId}, user {UserId}, amount {Amount}",
                request.OrderId, request.UserId, request.Amount);

            if (request.Amount <= 0)
            {
                throw new ArgumentException("Payment amount must be positive");
            }

            try
            {
                var account = await dbContext.Accounts
                    .FirstOrDefaultAsync(a => a.UserId == request.UserId);

                if (account == null)
                {
                    _logger.LogWarning("Account not found for user {UserId}", request.UserId);
                    throw new KeyNotFoundException($"Account not found for user {request.UserId}");
                }

                if (account.Balance < request.Amount)
                {
                    _logger.LogWarning("Insufficient balance for payment. User: {UserId}, Balance: {Balance}, Required: {Amount}",
                        request.UserId, account.Balance, request.Amount);
                    return false;
                }

                var transactionRecord = new Transaction
                {
                    Id = Guid.NewGuid(),
                    AccountId = account.Id,
                    Amount = -request.Amount,
                    Description = $"Payment for order {request.OrderId}",
                    Type = TransactionType.Payment,
                    CreatedAt = DateTime.UtcNow,
                    RelatedOrderId = request.OrderId
                };

                await dbContext.Transactions.AddAsync(transactionRecord);

                account.Balance -= request.Amount;
                account.UpdatedAt = DateTime.UtcNow;
                account.Version++;

                _logger.LogInformation("Payment successful for order {OrderId}, user {UserId}, new balance: {Balance}",
                    request.OrderId, request.UserId, account.Balance);

                return true;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Concurrency conflict during payment for order {OrderId}", request.OrderId);
                throw new Exception("A concurrency conflict occurred. Please try again.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during payment for order {OrderId}", request.OrderId);
                throw;
            }
        }
    }
}
