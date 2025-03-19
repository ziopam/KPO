namespace KR1.DomainObjects
{
    internal class Operation(int id, bool isIncome, int bankAccountId, decimal amount, DateOnly date, int categoryId, string description = "")
    {
        public int Id { get; private set; } = id;
        public bool IsIncome { get; set; } = isIncome;
        public int BankAccountId { get; set; } = bankAccountId;
        public decimal Amount { get; set; } = amount;
        public DateOnly Date { get; set; } = date;
        public int CategoryId { get; set; } = categoryId;
        public string Description { get; set; } = description;
    }
}
