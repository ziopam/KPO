using ConsoleTables;
using KR1.Database;

namespace KR1.Console_Interface.Commands
{
    internal class ShowCategoriesCommand(IFinanceDatabase financeDatabase) : ICommand
    {

        private readonly IFinanceDatabase financeDatabase = financeDatabase;

        public void Execute()
        {
            var categories = financeDatabase.GetCategories();

            if (!categories.Any())
            {
                Console.WriteLine("Категории отсутствуют");
                return;
            }

            ConsoleTable table = new("ID", "Название", "Тип");
            foreach (var category in categories)
            {
                table.AddRow(category.Id, category.Name, category.IsIncome ? "Доход" : "Расход");
            }
            table.Write();
        }
    }
}
