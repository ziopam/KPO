namespace KR1.DomainObjects
{
    internal class BankAccount(Int64 id, string name, decimal balance)
    {
        public Int64 Id { get; private set; } = id;
        public string Name { get; set; } = name;
        public decimal Balance { get; private set; } = balance;

        public void UpdateBalance(decimal amount)
        {
            Balance += amount;
        }
    }
}
