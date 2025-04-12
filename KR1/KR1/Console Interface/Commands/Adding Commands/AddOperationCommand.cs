using KR1.Database;
using KR1.DomainObjects;
using System.Text;

namespace KR1.Console_Interface.Commands
{
    internal class AddOperationCommand(IFinanceDatabase financeDatabase) : ICommand
    {

        private readonly IFinanceDatabase financeDatabase = financeDatabase;

        public void Execute()
        {
            var accounts = financeDatabase.GetAccounts();
            var categories = financeDatabase.GetCategories();

            if (!accounts.Any())
            {
                Console.WriteLine("Сначала добавьте хотя бы один счет");
                return;
            }

            if (!categories.Any())
            {
                Console.WriteLine("Сначала добавьте хотя бы одну категорию");
                return;
            }

            BankAccount selectedAccount = DataGetterFacade.GetBankAccountFromUser("Выберите счет: ", accounts);
            Category selectedCategory = DataGetterFacade.GetCategoryFromUser("Выберите категорию: ", categories);
            bool isIncome = selectedCategory.IsIncome;
            decimal amount = DataGetterFacade.GetDecimalFromUser("Введите сумму операции: ", true);

            DateOnly date = DataGetterFacade.GetDateFromUser("Введите дату операции.");

            string description = DataGetterFacade.GetStringFromUser("Введите описание операции (может быть пустым): ", true);

            StringBuilder sb = new();
            sb.AppendLine($"Счет: {selectedAccount.Name} (ID: {selectedAccount.Id})");
            sb.AppendLine($"Категория: {selectedCategory.Name} (ID: {selectedCategory.Id})");
            sb.AppendLine($"Тип операции: {(isIncome ? "Доход" : "Расход")}");
            sb.AppendLine($"Сумма: {amount}");
            sb.AppendLine($"Дата: {date}");
            sb.AppendLine($"Описание: {description}");
            sb.Append("Вы уверены, что хотите добавить эту операцию?");

            if (DataGetterFacade.GetBoolFromUser(sb.ToString()))
            {
                financeDatabase.AddOperation(isIncome, selectedAccount.Id, amount, date, selectedCategory.Id, description);
                Console.WriteLine("Операция успешно добавлена");
                return;
            }
            Console.WriteLine("Отмена добавления операции");
        }
    }
}
