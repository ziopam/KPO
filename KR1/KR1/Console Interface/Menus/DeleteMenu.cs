using KR1.Console_Interface.Commands;
using KR1.Console_Interface.Commands.Deleting_Commands;
using KR1.Database;

namespace KR1.Console_Interface.Menus
{
    internal class DeleteMenu : Menu
    {
        public DeleteMenu(IFinanceDatabase financeDatabase, Menu mainMenu)
        {
            AddItem(new MenuItem { Title = "Удалить счёт", Command = new FinalCommand(new DeleteAccountCommand(financeDatabase), mainMenu) });
            AddItem(new MenuItem { Title = "Удалить категорию", Command = new FinalCommand(new DeleteCategoryCommand(financeDatabase), mainMenu) });
            AddItem(new MenuItem { Title = "Удалить операцию", Command = new FinalCommand(new DeleteOperationCommand(financeDatabase), mainMenu) });
        }
    }
}
