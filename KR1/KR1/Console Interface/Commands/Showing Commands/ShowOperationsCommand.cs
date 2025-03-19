using ConsoleTables;
using KR1.Database;

namespace KR1.Console_Interface.Commands
{
    internal class ShowOperationsCommand(IFinanceDatabase financeDatabase) : ICommand
    {
        private readonly IFinanceDatabase financeDatabase = financeDatabase;

        public void Execute()
        {
            var operations = financeDatabase.GetOperations().OrderBy(o => o.Date);

            if (!operations.Any())
            {
                Console.WriteLine("Операции отсутствуют");
                return;
            }

            ConsoleTable table = new("ID", "Cчет (ID)", "Категория (ID)", "Сумма", "Дата", "Тип операции", "Описание");

            foreach (var operation in operations)
            {
                table.AddRow(operation.Id, operation.BankAccountId, operation.CategoryId, operation.Amount,
                    operation.Date, operation.IsIncome ? "Доход" : "Расход", operation.Description);
            }
            table.Write();
        }
    }
}
