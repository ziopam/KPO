namespace KR3.Shared.Models
{
    public class CreateOrderRequest
    {
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public Guid UserId { get; set; }
    }
}
