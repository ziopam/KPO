using KR1.Console_Interface.Commands;
using KR1.Console_Interface.Commands.Export_Commands;
using KR1.Database;
using KR1.DomainObjects;

namespace KR1.Console_Interface.Menus
{
    internal class ExportMenu : Menu
    {
        public ExportMenu(IFinanceDatabase financeDatabase, Menu mainMenu)
        {
            List<BankAccount> bankAccounts = financeDatabase.GetAccounts().ToList();
            List<Category> categories = financeDatabase.GetCategories().ToList();
            List<Operation> operations = financeDatabase.GetOperations().ToList();

            Header = "Выберите таблицу для экспорта данных (файл будет создан рядом с исполняемым файлом): ";

            AddItem(new MenuItem { Title = "Счета", Command = new FinalCommand(new ExportCommand<BankAccount>(bankAccounts), mainMenu) });
            AddItem(new MenuItem { Title = "Категории", Command = new FinalCommand(new ExportCommand<Category>(categories), mainMenu) });
            AddItem(new MenuItem { Title = "Операции", Command = new FinalCommand(new ExportCommand<Operation>(operations), mainMenu) });
        }
    }
}
