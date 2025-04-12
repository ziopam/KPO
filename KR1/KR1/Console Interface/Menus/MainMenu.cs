using KR1.Console_Interface.Commands;
using KR1.Database;

namespace KR1.Console_Interface.Menus
{
    internal class MainMenu : Menu
    {
        private readonly Menu addMenu;
        private readonly Menu showMenu;
        private readonly Menu deleteMenu;
        private readonly Menu editMenu;
        private readonly Menu exportMenu;

        public MainMenu(IFinanceDatabase financeDatabase)
        {
            addMenu = new AddMenu(financeDatabase, this);
            showMenu = new ShowMenu(financeDatabase, this);
            deleteMenu = new DeleteMenu(financeDatabase, this);
            editMenu = new EditMenu(financeDatabase, this);
            exportMenu = new ExportMenu(financeDatabase, this);

            AddItem(new MenuItem { Title = "Добавить в базу данных", Command = new OpenMenuCommand(addMenu) });
            AddItem(new MenuItem { Title = "Показать базу данных", Command = new OpenMenuCommand(showMenu) });
            AddItem(new MenuItem { Title = "Удалить из базы данных", Command = new OpenMenuCommand(deleteMenu) });
            AddItem(new MenuItem { Title = "Редактировать базу данных", Command = new OpenMenuCommand(editMenu) });
            AddItem(new MenuItem { Title = "Экспорт базы данных", Command = new OpenMenuCommand(exportMenu) });
        }
    }
}
