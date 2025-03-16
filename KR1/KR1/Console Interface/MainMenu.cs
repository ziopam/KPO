using KR1.Console_Interface.Commands;
using KR1.Database;

namespace KR1.Console_Interface
{
    internal class MainMenu : Menu
    {

        public MainMenu(IFinanceDatabase financeDatabase)
        {
            AddItem(new MenuItem { Title = "Добавить счёт", Command = new FinalCommand(new AddAccountCommand(financeDatabase), this) });
            AddItem(new MenuItem { Title = "Добавить категорию", Command = new AddAccountCommand(financeDatabase) });
            AddItem(new MenuItem { Title = "Добавить операцию", Command = new AddAccountCommand(financeDatabase) });
            AddItem(new MenuItem { Title = "Посмотреть счета", Command = new FinalCommand(new ShowAccountsCommand(financeDatabase), this) });
            AddItem(new MenuItem { Title = "Посмотреть категории", Command = new AddAccountCommand(financeDatabase) });
            AddItem(new MenuItem { Title = "Посмотреть операции", Command = new AddAccountCommand(financeDatabase) });
        }
    }
}
