namespace KR1.Console_Interface
{
    internal class MenuItem
    {
        public required string Title { get; set; }
        public required ICommand Command { get; set; }
    }
}
