namespace KR1.DomainObjects
{
    internal class Operation(int id, bool isIncome, int bankAccountId, decimal amount, DateTime date, int categoryId, string description = "")
    {
        public int Id { get; private set; } = id;
        public bool IsStonks { get; private set; } = isIncome;
        public int BankAccountId { get; private set; } = bankAccountId;
        public decimal Amount { get; private set; } = amount;
        public DateTime Date { get; private set; } = date;
        public int CategoryId { get; private set; } = categoryId;
        public string Description { get; private set; } = description;
    }
}
