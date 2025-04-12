namespace KR1.DomainObjects
{
    internal class Operation(Int64 id, bool isIncome, Int64 bankAccountId, decimal amount, DateOnly date, Int64 categoryId, string description = "")
    {
        public Int64 Id { get; private set; } = id;
        public bool IsIncome { get; set; } = isIncome;
        public Int64 BankAccountId { get; set; } = bankAccountId;
        public decimal Amount { get; set; } = amount;
        public DateOnly Date { get; set; } = date;
        public Int64 CategoryId { get; set; } = categoryId;
        public string Description { get; set; } = description;
    }
}
