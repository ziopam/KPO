namespace KR3.Shared.Models
{
    public class AccountResponse
    {
        public Guid AccountId { get; set; }
        public Guid UserId { get; set; }
        public decimal Balance { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
