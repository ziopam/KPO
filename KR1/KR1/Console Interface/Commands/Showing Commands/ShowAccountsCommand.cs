namespace KR1.Console_Interface.Commands
{
    using ConsoleTables;
    using KR1.Database;

    internal class ShowAccountsCommand(IFinanceDatabase financeDatabase) : ICommand
    {
        private readonly IFinanceDatabase financeDatabase = financeDatabase;

        public void Execute()
        {
            var accounts = financeDatabase.GetAccounts();

            if (!accounts.Any())
            {
                Console.WriteLine("Счета отсутствуют");
                return;
            }

            ConsoleTable table = new("ID", "Название", "Баланс");
            foreach (var account in accounts)
            {
                table.AddRow(account.Id, account.Name, account.Balance);
            }
            table.Write();
        }
    }
}
