using KR1.Database;
using System.Text;

namespace KR1.Console_Interface.Commands
{
    internal class AddAccountCommand(IFinanceDatabase financeDatabase) : ICommand
    {
        private readonly IFinanceDatabase _financeDatabase = financeDatabase;

        public void Execute()
        {
            String name = DataGetterFacade.GetStringFromUser("Введите название счёта: ");
            decimal balance = DataGetterFacade.GetDecimalFromUser("Введите начальный баланс: ");
            Console.Clear();

            StringBuilder sb = new();
            sb.AppendLine($"Название счёта: {name}");
            sb.AppendLine($"Начальный баланс: {balance}");
            sb.Append("Вы уверены, что хотите добавить этот счёт?");

            if (DataGetterFacade.GetBoolFromUser(sb.ToString()))
            {
                _financeDatabase.AddAccount(name, balance);
                Console.WriteLine("Счёт успешно добавлен");
                return;
            }

            Console.WriteLine("Отмена добавления счета");
        }
    }
}
