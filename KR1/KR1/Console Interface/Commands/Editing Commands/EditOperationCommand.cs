using KR1.Database;
using KR1.DomainObjects;
using System.Text;

namespace KR1.Console_Interface.Commands.Editing_Commands
{
    internal class EditOperationCommand(IFinanceDatabase financeDatabase) : ICommand
    {
        private readonly IFinanceDatabase financeDatabase = financeDatabase;

        public void Execute()
        {
            var operations = financeDatabase.GetOperations();

            if (!operations.Any())
            {
                Console.WriteLine("Нет данных для редактирования");
                return;
            }

            Operation operation = DataGetterFacade.GetOperationFromUser("Выберите операцию для редактирования: ", operations);

            int selectedField = DataGetterFacade.GetOptionsFromUser("Выберите поле для редактирования: ", [
                "Счет, связанный с операцией",
                "Категория",
                "Сумма",
                "Дата",
                "Описание"
            ]);

            StringBuilder sb = new();
            if (selectedField == 0)
            {
                var accounts = financeDatabase.GetAccounts();
                if (!accounts.Any())
                {
                    Console.WriteLine("Нет счетов для связывания с операцией");
                    return;
                }

                BankAccount bankAccount = DataGetterFacade.GetBankAccountFromUser("Выберите новый счет: ", accounts);

                sb.AppendLine($"ID: {operation.Id}");
                sb.AppendLine($"Счет, связанный с операцией (ID): {operation.BankAccountId} -> {bankAccount.Id}");
                sb.AppendLine($"Категория (ID): {operation.CategoryId}");
                sb.AppendLine($"Сумма: {operation.Amount}");
                sb.AppendLine($"Дата: {operation.Date}");
                sb.AppendLine($"Тип операции: {(operation.IsIncome ? "Доход" : "Расход")}");
                sb.AppendLine($"Описание: {operation.Description}");
                sb.Append("Вы уверены, что хотите изменить счет, связанный с операцией?");

                if (DataGetterFacade.GetBoolFromUser(sb.ToString()))
                {
                    financeDatabase.UpdateOperationBankAccountId(operation.Id, bankAccount.Id);
                    Console.WriteLine("Операция успешно обновлена");
                    return;
                }
            }
            else if (selectedField == 1)
            {
                var categories = financeDatabase.GetCategories();
                if (!categories.Any())
                {
                    Console.WriteLine("Нет категорий для связывания с операцией");
                    return;
                }

                Category category = DataGetterFacade.GetCategoryFromUser("Выберите новую категорию: ", categories);

                sb.AppendLine($"ID: {operation.Id}");
                sb.AppendLine($"Счет, связанный с операцией (ID): {operation.BankAccountId}");
                sb.AppendLine($"Категория (ID): {operation.CategoryId} -> {category.Id}");
                sb.AppendLine($"Сумма: {operation.Amount}");
                sb.AppendLine($"Дата: {operation.Date}");
                sb.AppendLine($"Тип операции: {(operation.IsIncome ? "Доход" : "Расход")}");
                sb.AppendLine($"Описание: {operation.Description}");
                sb.Append("Вы уверены, что хотите изменить категорию операции?");

                if (DataGetterFacade.GetBoolFromUser(sb.ToString()))
                {
                    financeDatabase.UpdateOperationCategoryId(operation.Id, category.Id);
                    Console.WriteLine("Операция успешно обновлена");
                    return;
                }
            }
            else if (selectedField == 2)
            {
                decimal amount = DataGetterFacade.GetDecimalFromUser("Введите новую сумму: ", true);

                sb.AppendLine($"ID: {operation.Id}");
                sb.AppendLine($"Счет, связанный с операцией (ID): {operation.BankAccountId}");
                sb.AppendLine($"Категория (ID): {operation.CategoryId}");
                sb.AppendLine($"Сумма: {operation.Amount} -> {amount}");
                sb.AppendLine($"Дата: {operation.Date}");
                sb.AppendLine($"Тип операции: {(operation.IsIncome ? "Доход" : "Расход")}");
                sb.AppendLine($"Описание: {operation.Description}");
                sb.Append("Вы уверены, что хотите изменить категорию операции?");

                if (DataGetterFacade.GetBoolFromUser(sb.ToString()))
                {
                    financeDatabase.UpdateOperationAmount(operation.Id, amount);
                    Console.WriteLine("Операция успешно обновлена");
                    return;
                }
            }
            else if (selectedField == 3)
            {
                DateOnly date = DataGetterFacade.GetDateFromUser("Введите новую дату: ");

                sb.AppendLine($"ID: {operation.Id}");
                sb.AppendLine($"Счет, связанный с операцией (ID): {operation.BankAccountId}");
                sb.AppendLine($"Категория (ID): {operation.CategoryId}");
                sb.AppendLine($"Сумма: {operation.Amount}");
                sb.AppendLine($"Дата: {operation.Date} -> {date}");
                sb.AppendLine($"Тип операции: {(operation.IsIncome ? "Доход" : "Расход")}");
                sb.AppendLine($"Описание: {operation.Description}");
                sb.Append("Вы уверены, что хотите изменить дату операции?");

                if (DataGetterFacade.GetBoolFromUser(sb.ToString()))
                {
                    financeDatabase.UpdateOperationDate(operation.Id, date);
                    Console.WriteLine("Операция успешно обновлена");
                    return;
                }
            }
            else
            {
                string description = DataGetterFacade.GetStringFromUser("Введите новое описание: ", true);

                sb.AppendLine($"ID: {operation.Id}");
                sb.AppendLine($"Счет, связанный с операцией (ID): {operation.BankAccountId}");
                sb.AppendLine($"Категория (ID): {operation.CategoryId}");
                sb.AppendLine($"Сумма: {operation.Amount}");
                sb.AppendLine($"Дата: {operation.Date}");
                sb.AppendLine($"Тип операции: {(operation.IsIncome ? "Доход" : "Расход")}");
                sb.AppendLine($"Описание: {operation.Description} -> {description}");
                sb.Append("Вы уверены, что хотите изменить описание операции?");

                if (DataGetterFacade.GetBoolFromUser(sb.ToString()))
                {
                    financeDatabase.UpdateOperationDescription(operation.Id, description);
                    Console.WriteLine("Операция успешно обновлена");
                    return;
                }
            }

            Console.WriteLine("Отмена редактирования операции");
        }
    }
}
