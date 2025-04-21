using KR1.Database;
using System.Text;

namespace KR1.Console_Interface.Commands.Deleting_Commands
{
    internal class DeleteAccountCommand(IFinanceDatabase financeDatabase) : ICommand
    {
        private readonly IFinanceDatabase financeDatabase = financeDatabase;

        public void Execute()
        {
            var accounts = financeDatabase.GetAccounts();

            if (!accounts.Any())
            {
                Console.WriteLine("Нет данных для удаления");
                return;
            }

            var selectedAccount = DataGetterFacade.GetBankAccountFromUser("Выберите счет для удаления: ", accounts);

            StringBuilder sb = new();
            sb.AppendLine($"ID: {selectedAccount.Id}");
            sb.AppendLine($"Название счёта: {selectedAccount.Name}");
            sb.AppendLine($"Баланс: {selectedAccount.Balance}");
            sb.Append("Вы уверены, что хотите удалить этот счёт?");

            if (DataGetterFacade.GetBoolFromUser(sb.ToString()))
            {
                financeDatabase.RemoveAccount(selectedAccount.Id);
                Console.WriteLine("Счёт успешно удалён");
                return;
            }

            Console.WriteLine("Отмена удаления счета");
        }
    }
}
