namespace KR1.Console_Interface.Commands
{
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

            Console.WriteLine("ID\tНазвание\tБаланс");

            foreach (var account in accounts)
            {
                Console.WriteLine($"{account.Id}\t{account.Name}\t{account.Balance}");
            }
        }
    }
}
