using KR1.Console_Interface.Commands;
using KR1.Database;

namespace KR1.Console_Interface.Menus
{
    internal class ShowMenu : Menu
    {
        public ShowMenu(IFinanceDatabase financeDatabase, Menu mainMenu)
        {
            AddItem(new MenuItem { Title = "Посмотреть счета", Command = new FinalCommand(new ShowAccountsCommand(financeDatabase), mainMenu) });
            AddItem(new MenuItem { Title = "Посмотреть категории", Command = new FinalCommand(new ShowCategoriesCommand(financeDatabase), mainMenu) });
            AddItem(new MenuItem { Title = "Посмотреть операции", Command = new FinalCommand(new ShowOperationsCommand(financeDatabase), mainMenu) });
        }
    }
}
