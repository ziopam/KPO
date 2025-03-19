using KR1.DomainObjects;

namespace KR1.Database
{
    internal class FinanceStorageProxy : IFinanceDatabase
    {
        private List<BankAccount> _accounts = [];
        private List<Category> _categories = [];
        private List<Operation> _operations = [];

        public void AddAccount(string name, decimal balance)
        {
            ArgumentNullException.ThrowIfNull(name);

            if (_accounts.Count == 0)
            {
                _accounts.Add(new BankAccount(1, name, balance));
            }
            else
            {
                _accounts.Add(new BankAccount(_accounts.Last().Id + 1, name, balance));
            }
        }

        public void AddCategory(string name, bool isIncome)
        {
            ArgumentNullException.ThrowIfNull(name);

            if (_categories.Count == 0)
            {
                _categories.Add(new Category(1, name, isIncome));
            }
            else
            {
                _categories.Add(new Category(_categories.Last().Id + 1, name, isIncome));
            }
        }

        public void AddOperation(bool isIncome, int bankAccountId, decimal amount, DateOnly date, int categoryId, string description = "")
        {
            ArgumentNullException.ThrowIfNull(description);

            int possible_id = _operations.Count == 0 ? 1 : _operations.Last().Id + 1;

            BankAccount? bankAccount = _accounts.Find(account => account.Id == bankAccountId) ?? throw new ArgumentException("Банка с переданным id не существует");

            if (_categories.Find(category => category.Id == categoryId) == null)
            {
                throw new ArgumentException("Категории с переданным id не существует");
            }


            bankAccount.UpdateBalance(isIncome ? amount : -amount);
            _operations.Add(new Operation(possible_id, isIncome, bankAccountId, amount, date, categoryId, description));
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

        public void RemoveAccount(int accountId)
        {
            _accounts = _accounts.Where(account => account.Id != accountId).ToList();
            _operations = _operations.Where(operation => operation.BankAccountId != accountId).ToList();
        }

        public void RemoveCategory(int categoryId)
        {
            Category? category = _categories.Find(category => category.Id == categoryId);

            if (category == null)
            {
                return;
            }

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

        public void RemoveOperation(int operationId)
        {
            Operation? operation = _operations.Find(operation => operation.Id == operationId);
            if (operation == null)
            {
                return;
            }

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

        public void UpdateAccountName(int accountId, string newName)
        {
            ArgumentNullException.ThrowIfNull(newName);
            BankAccount? account = _accounts.Find(account => account.Id == accountId);
            if (account == null)
            {
                return;
            }
            account.Name = newName;
        }

        public void UpdateCategoryName(int categoryId, string newName)
        {
            ArgumentNullException.ThrowIfNull(newName);
            Category? category = _categories.Find(category => category.Id == categoryId);
            if (category == null)
            {
                return;
            }
            category.Name = newName;
        }

        public void UpdateCategoryIsIncome(int categoryId, bool isIncome)
        {
            Category? category = _categories.Find(category => category.Id == categoryId);
            if (category == null || category.IsIncome == isIncome)
            {
                return;
            }

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

        public void UpdateOperationBankAccountId(int operationId, int newBankAccountId)
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

        public void UpdateOperationCategoryId(int operationId, int newCategoryId)
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

        public void UpdateOperationAmount(int operationId, decimal newAmount)
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

        public void UpdateOperationDate(int operationId, DateOnly newDate)
        {
            Operation? operation = _operations.Find(operation => operation.Id == operationId);
            if (operation == null)
            {
                return;
            }

            operation.Date = newDate;
        }

        public void UpdateOperationDescription(int operationId, string newDescription)
        {
            ArgumentNullException.ThrowIfNull(newDescription);

            Operation? operation = _operations.Find(operation => operation.Id == operationId);
            if (operation == null)
            {
                return;
            }
            operation.Description = newDescription;
        }
    }
}
