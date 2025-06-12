namespace KR3.Shared.Models
{
    public class OrderResponse
    {
        public Guid OrderId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public OrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid UserId { get; set; }
    }
}
