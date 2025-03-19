using KR1.Console_Interface.Commands;
using KR1.Database;

namespace KR1.Console_Interface.Menus
{
    internal class AddMenu : Menu
    {
        public AddMenu(IFinanceDatabase financeDatabase, Menu mainMenu)
        {
            AddItem(new MenuItem { Title = "Добавить счёт", Command = new FinalCommand(new AddAccountCommand(financeDatabase), mainMenu) });
            AddItem(new MenuItem { Title = "Добавить категорию", Command = new FinalCommand(new AddCategoryCommand(financeDatabase), mainMenu) });
            AddItem(new MenuItem { Title = "Добавить операцию", Command = new FinalCommand(new AddOperationCommand(financeDatabase), mainMenu) });
        }
    }
}
