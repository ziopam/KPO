using KR1.DomainObjects;

namespace KR1.Database
{
    internal interface IFinanceDatabase
    {
        void AddAccount(string name, decimal balance);
        void AddCategory(string name, bool isIncome);
        void AddOperation(bool isIncome, Int64 bankAccountId, decimal amount, DateOnly date, Int64 categoryId, string description = "");

        IEnumerable<BankAccount> GetAccounts();
        IEnumerable<Category> GetCategories();
        IEnumerable<Operation> GetOperations();

        void RemoveAccount(Int64 accountId);
        void RemoveCategory(Int64 categoryId);
        void RemoveOperation(Int64 operationId);

        void UpdateAccountName(Int64 accountId, string newName);
        void UpdateCategoryName(Int64 categoryId, string newName);
        void UpdateCategoryIsIncome(Int64 categoryId, bool isIncome);
        void UpdateOperationBankAccountId(Int64 operationId, Int64 newBankAccountId);
        void UpdateOperationCategoryId(Int64 operationId, Int64 newCategoryId);
        void UpdateOperationAmount(Int64 operationId, decimal newAmount);
        void UpdateOperationDate(Int64 operationId, DateOnly newDate);
        void UpdateOperationDescription(Int64 operationId, string newDescription);
    }
}
