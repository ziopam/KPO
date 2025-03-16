using KR1.DomainObjects;

namespace KR1.Database
{
    internal class FinanceStorage : IFinanceDatabase
    {
        private readonly List<BankAccount> _accounts = [];
        private readonly List<Category> _categories = [];
        private readonly List<Operation> _operations = [];

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

        public void AddOperation(int id, bool isIncome, int bankAccountId, decimal amount, DateTime date, int categoryId, string description = "")
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
    }
}
