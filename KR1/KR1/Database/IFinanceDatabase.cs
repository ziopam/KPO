using KR1.DomainObjects;

namespace KR1.Database
{
    internal interface IFinanceDatabase
    {
        void AddAccount(string name, decimal balance);
        void AddCategory(string name, bool isIncome);
        void AddOperation(bool isIncome, int bankAccountId, decimal amount, DateOnly date, int categoryId, string description = "");

        IEnumerable<BankAccount> GetAccounts();
        IEnumerable<Category> GetCategories();
        IEnumerable<Operation> GetOperations();

        void RemoveAccount(int accountId);
        void RemoveCategory(int categoryId);
        void RemoveOperation(int operationId);

        void UpdateAccountName(int accountId, string newName);
        void UpdateCategoryName(int categoryId, string newName);
        void UpdateCategoryIsIncome(int categoryId, bool isIncome);
        void UpdateOperationBankAccountId(int operationId, int newBankAccountId);
        void UpdateOperationCategoryId(int operationId, int newCategoryId);
        void UpdateOperationAmount(int operationId, decimal newAmount);
        void UpdateOperationDate(int operationId, DateOnly newDate);
        void UpdateOperationDescription(int operationId, string newDescription);
    }
}
