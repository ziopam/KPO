namespace KR3.Shared.Models
{
    public class DepositRequest
    {
        public Guid UserId { get; set; }
        public decimal Amount { get; set; }
    }
}
