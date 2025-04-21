using KR1.Console_Interface.Commands;
using KR1.Console_Interface.Commands.Editing_Commands;
using KR1.Database;

namespace KR1.Console_Interface.Menus
{
    internal class EditMenu : Menu
    {
        public EditMenu(IFinanceDatabase financeDatabase, Menu mainMenu)
        {
            AddItem(new MenuItem { Title = "Изменить название счёта", Command = new FinalCommand(new EditAccountCommand(financeDatabase), mainMenu) });
            AddItem(new MenuItem { Title = "Изменить категорию", Command = new FinalCommand(new EditCategoryCommand(financeDatabase), mainMenu) });
            AddItem(new MenuItem { Title = "Изменить операцию", Command = new FinalCommand(new EditOperationCommand(financeDatabase), mainMenu) });
        }
    }
}
