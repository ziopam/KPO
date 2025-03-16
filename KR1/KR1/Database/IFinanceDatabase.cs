using KR1.DomainObjects;

namespace KR1.Database
{
    internal interface IFinanceDatabase
    {
        void AddAccount(string name, decimal balance);
        void AddCategory(string name, bool isIncome);
        void AddOperation(int id, bool isIncome, int bankAccountId, decimal amount, DateTime date, int categoryId, string description = "");

        IEnumerable<BankAccount> GetAccounts();
        IEnumerable<Category> GetCategories();
        IEnumerable<Operation> GetOperations();
    }
}
