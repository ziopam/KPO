using KR1.Database;

namespace KR1.Console_Interface.Commands
{
    internal class AddAccountCommand(IFinanceDatabase financeDatabase) : ICommand
    {
        private readonly IFinanceDatabase _financeDatabase = financeDatabase;

        public void Execute()
        {
            String name = DataGetter.GetStringFromUser("Введите название счёта: ");
            decimal balance = DataGetter.GetDecimalFromUser("Введите начальный баланс: ");
            _financeDatabase.AddAccount(name, balance);
            Console.WriteLine("Счёт успешно добавлен");
        }
    }
}
