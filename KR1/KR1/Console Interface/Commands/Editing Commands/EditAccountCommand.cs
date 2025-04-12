using KR1.Database;
using KR1.DomainObjects;
using System.Text;

namespace KR1.Console_Interface.Commands.Editing_Commands
{
    internal class EditAccountCommand(IFinanceDatabase financeDatabase) : ICommand
    {
        private readonly IFinanceDatabase financeDatabase = financeDatabase;

        public void Execute()
        {
            var accounts = financeDatabase.GetAccounts();

            if (!accounts.Any())
            {
                Console.WriteLine("Нет данных для редактирования");
                return;
            }

            BankAccount account = DataGetterFacade.GetBankAccountFromUser("Выберите счет для редактирования: ", accounts);
            string newName = DataGetterFacade.GetStringFromUser("Введите новое название счета: ");

            StringBuilder sb = new();
            sb.AppendLine($"ID: {account.Id}");
            sb.AppendLine($"Название: {account.Name} -> {newName}");
            sb.AppendLine($"Баланс: {account.Balance}");
            sb.Append("Вы уверены, что хотите изменить название этого счета?");

            if (DataGetterFacade.GetBoolFromUser(sb.ToString()))
            {
                account.Name = newName;
                financeDatabase.UpdateAccountName(account.Id, newName);
                Console.WriteLine("Счет успешно изменен");
                return;
            }

            Console.WriteLine("Отмена изменения счета");
        }
    }
}
