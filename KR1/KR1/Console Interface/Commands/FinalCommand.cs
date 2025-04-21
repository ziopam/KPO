namespace KR1.Console_Interface.Commands
{
    using KR1.Console_Interface.Menus;

    internal class FinalCommand(ICommand command, Menu menu) : ICommand
    {
        private readonly ICommand command = command;
        private readonly Menu menu = menu;

        public void Execute()
        {
            Console.Clear();
            command.Execute();
            Console.WriteLine("Нажмите любую клавишу для продолжения...");
            Console.ReadKey();
            menu.Display();
        }
    }
}
