namespace KR1.Console_Interface.Commands
{
    using KR1.Console_Interface.Menus;
    internal class OpenMenuCommand(Menu menu) : ICommand
    {
        public void Execute()
        {
            menu.Display();
        }
    }
}
