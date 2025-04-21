using KR1.DomainObjects;

namespace KR1.Database
{
    internal class FinanceStorageProxy : IFinanceDatabase
    {
        private List<BankAccount> _accounts = [];
        private List<Category> _categories = [];
        private List<Operation> _operations = [];

        private readonly FinanceDatabase _financeDatabase;

        public FinanceStorageProxy(FinanceDatabase financeDatabase)
        {
            _financeDatabase = financeDatabase;
            _accounts = (List<BankAccount>)_financeDatabase.GetAccounts();
            _categories = (List<Category>)_financeDatabase.GetCategories();
            _operations = (List<Operation>)_financeDatabase.GetOperations();
        }

        public void AddAccount(string name, decimal balance)
        {
            ArgumentNullException.ThrowIfNull(name);

            Int64 id = _financeDatabase.GetNextId("BankAccounts");
            _financeDatabase.AddAccount(name, balance);
            _accounts.Add(new BankAccount(id, name, balance));
        }

        public void AddCategory(string name, bool isIncome)
        {
            ArgumentNullException.ThrowIfNull(name);

            Int64 id = _financeDatabase.GetNextId("Categories");
            _financeDatabase.AddCategory(name, isIncome);
            _categories.Add(new Category(id, name, isIncome));
        }

        public void AddOperation(bool isIncome, Int64 bankAccountId, decimal amount, DateOnly date, Int64 categoryId, string description = "")
        {
            ArgumentNullException.ThrowIfNull(description);

            Int64 id = _financeDatabase.GetNextId("Operations");
            _financeDatabase.AddOperation(isIncome, bankAccountId, amount, date, categoryId, description);

            BankAccount? bankAccount = _accounts.Find(account => account.Id == bankAccountId) ?? throw new ArgumentException("Банка с переданным id не существует");

            if (_categories.Find(category => category.Id == categoryId) == null)
            {
                throw new ArgumentException("Категории с переданным id не существует");
            }

            bankAccount.UpdateBalance(isIncome ? amount : -amount);
            _operations.Add(new Operation(id, isIncome, bankAccountId, amount, date, categoryId, description));
        }

        public IEnumerable<BankAccount> GetAccounts()
        {
            return _accounts;
        }

        public IEnumerable<Category> GetCategories()
        {
            return _categories;
        }

        public IEnumerable<Operation> GetOperations()
        {
            return _operations;
        }

        public void RemoveAccount(Int64 accountId)
        {
            _financeDatabase.RemoveAccount(accountId);
            _accounts = _accounts.Where(account => account.Id != accountId).ToList();
            _operations = _operations.Where(operation => operation.BankAccountId != accountId).ToList();
        }

        public void RemoveCategory(Int64 categoryId)
        {
            Category? category = _categories.Find(category => category.Id == categoryId);

            if (category == null)
            {
                return;
            }

            _financeDatabase.RemoveCategory(categoryId);

            var operationsToRemove = _operations.Where(operation => operation.CategoryId == categoryId).ToList();
            foreach (var operation in operationsToRemove)
            {
                if (category.IsIncome)
                {
                    _accounts.Find(account => account.Id == operation.BankAccountId)?.UpdateBalance(-operation.Amount);
                }
                else
                {
                    _accounts.Find(account => account.Id == operation.BankAccountId)?.UpdateBalance(operation.Amount);
                }
                _operations.Remove(operation);
            }
            _categories.Remove(category);
        }

        public void RemoveOperation(Int64 operationId)
        {
            Operation? operation = _operations.Find(operation => operation.Id == operationId);
            if (operation == null)
            {
                return;
            }

            _financeDatabase.RemoveOperation(operationId);

            if (_categories.Find(category => category.Id == operation.CategoryId)?.IsIncome == true)
            {
                _accounts.Find(account => account.Id == operation.BankAccountId)?.UpdateBalance(-operation.Amount);
            }
            else
            {
                _accounts.Find(account => account.Id == operation.BankAccountId)?.UpdateBalance(operation.Amount);
            }
            _operations.Remove(operation);
        }

        public void UpdateAccountName(Int64 accountId, string newName)
        {
            ArgumentNullException.ThrowIfNull(newName);

            BankAccount? account = _accounts.Find(account => account.Id == accountId);
            if (account == null)
            {
                return;
            }

            _financeDatabase.UpdateAccountName(accountId, newName);
            account.Name = newName;
        }

        public void UpdateCategoryName(Int64 categoryId, string newName)
        {
            ArgumentNullException.ThrowIfNull(newName);
            Category? category = _categories.Find(category => category.Id == categoryId);
            if (category == null)
            {
                return;
            }

            _financeDatabase.UpdateCategoryName(categoryId, newName);
            category.Name = newName;
        }

        public void UpdateCategoryIsIncome(Int64 categoryId, bool isIncome)
        {
            Category? category = _categories.Find(category => category.Id == categoryId);
            if (category == null || category.IsIncome == isIncome)
            {
                return;
            }

            _financeDatabase.UpdateCategoryIsIncome(categoryId, isIncome);
            category.IsIncome = isIncome;

            var operationsToUpdate = _operations.Where(operation => operation.CategoryId == categoryId).ToList();
            foreach (var operation in operationsToUpdate)
            {
                if (category.IsIncome)
                {
                    _accounts.Find(account => account.Id == operation.BankAccountId)?.UpdateBalance(2 * operation.Amount);
                }
                else
                {
                    _accounts.Find(account => account.Id == operation.BankAccountId)?.UpdateBalance(2 * (-operation.Amount));
                }
                operation.IsIncome = isIncome;
            }
        }

        public void UpdateOperationBankAccountId(Int64 operationId, Int64 newBankAccountId)
        {
            Operation? operation = _operations.Find(operation => operation.Id == operationId);
            if (operation == null)
            {
                return;
            }

            BankAccount? oldAccount = _accounts.Find(account => account.Id == operation.BankAccountId);
            BankAccount? newAccount = _accounts.Find(account => account.Id == newBankAccountId);

            if (oldAccount == null || newAccount == null)
            {
                return;
            }

            _financeDatabase.UpdateOperationBankAccountId(operationId, newBankAccountId);

            if (operation.IsIncome)
            {
                oldAccount.UpdateBalance(-operation.Amount);
                newAccount.UpdateBalance(operation.Amount);
            }
            else
            {
                oldAccount.UpdateBalance(operation.Amount);
                newAccount.UpdateBalance(-operation.Amount);
            }

            operation.BankAccountId = newBankAccountId;
        }

        public void UpdateOperationCategoryId(Int64 operationId, Int64 newCategoryId)
        {
            Operation? operation = _operations.Find(operation => operation.Id == operationId);
            if (operation == null)
            {
                return;
            }

            Category? oldCategory = _categories.Find(category => category.Id == operation.CategoryId);
            Category? newCategory = _categories.Find(category => category.Id == newCategoryId);

            if (oldCategory == null || newCategory == null)
            {
                return;
            }

            _financeDatabase.UpdateOperationCategoryId(operationId, newCategoryId);

            if (oldCategory.IsIncome)
            {
                _accounts.Find(account => account.Id == operation.BankAccountId)?.UpdateBalance(-operation.Amount);
            }
            else
            {
                _accounts.Find(account => account.Id == operation.BankAccountId)?.UpdateBalance(operation.Amount);
            }

            if (newCategory.IsIncome)
            {
                _accounts.Find(account => account.Id == operation.BankAccountId)?.UpdateBalance(operation.Amount);
            }
            else
            {
                _accounts.Find(account => account.Id == operation.BankAccountId)?.UpdateBalance(-operation.Amount);
            }

            operation.CategoryId = newCategoryId;
            operation.IsIncome = newCategory.IsIncome;
        }

        public void UpdateOperationAmount(Int64 operationId, decimal newAmount)
        {
            if (newAmount <= 0)
            {
                throw new ArgumentException("Сумма операции должна быть положительной");
            }

            Operation? operation = _operations.Find(operation => operation.Id == operationId);
            if (operation == null)
            {
                return;
            }

            _financeDatabase.UpdateOperationAmount(operationId, newAmount);

            BankAccount? account = _accounts.Find(account => account.Id == operation.BankAccountId);

            if (operation.IsIncome)
            {
                account?.UpdateBalance(newAmount - operation.Amount);
            }
            else
            {
                account?.UpdateBalance(operation.Amount - newAmount);
            }

            operation.Amount = newAmount;
        }

        public void UpdateOperationDate(Int64 operationId, DateOnly newDate)
        {
            Operation? operation = _operations.Find(operation => operation.Id == operationId);
            if (operation == null)
            {
                return;
            }

            _financeDatabase.UpdateOperationDate(operationId, newDate);
            operation.Date = newDate;
        }

        public void UpdateOperationDescription(Int64 operationId, string newDescription)
        {
            ArgumentNullException.ThrowIfNull(newDescription);

            Operation? operation = _operations.Find(operation => operation.Id == operationId);
            if (operation == null)
            {
                return;
            }

            _financeDatabase.UpdateOperationDescription(operationId, newDescription);
            operation.Description = newDescription;
        }
    }
}
