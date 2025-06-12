using KR3.Shared.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KR3.OrderService.Models
{
    public class Order
    {
        [Key]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public OrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public OrderResponse ToDto()
        {
            return new OrderResponse
            {
                OrderId = Id,
                UserId = UserId,
                Amount = Amount,
                Description = Description,
                Status = Status,
                CreatedAt = CreatedAt
            };
        }
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
}
