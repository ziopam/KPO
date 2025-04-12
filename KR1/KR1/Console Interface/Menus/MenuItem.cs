namespace KR1.Console_Interface.Menus
{
    internal class MenuItem
    {
        public required string Title { get; set; }
        public ICommand? Command { get; set; }
    }
}
