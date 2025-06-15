using KR3.Shared.Models;
using System.ComponentModel.DataAnnotations;

namespace KR3.PaymentService.Models
{
    public class Account
    {
        [Key]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public decimal Balance { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public long Version { get; set; }

        public AccountResponse ToDto()
        {
            return new AccountResponse
            {
                AccountId = Id,
                UserId = UserId,
                Balance = Balance,
                CreatedAt = CreatedAt
            };
        }
    }

    public class Transaction
    {
        [Key]
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public decimal Amount { get; set; }
        public required string Description { get; set; }
        public TransactionType Type { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid? RelatedOrderId { get; set; }
    }

    public enum TransactionType
    {
        Deposit,
        Payment
    }
    public class OutboxMessage
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Type { get; set; } = string.Empty;
        public string Data { get; set; } = string.Empty;
        public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
        public DateTime? ProcessedUtc { get; set; }
        public string Error { get; set; } = string.Empty;
    }
    public class InboxMessage
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid MessageId { get; set; }
        public string Data { get; set; } = string.Empty;
        public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
        public DateTime? ProcessedUtc { get; set; }
        public string Error { get; set; } = string.Empty;
    }
}
