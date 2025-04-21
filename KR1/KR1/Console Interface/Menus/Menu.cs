namespace KR1.Console_Interface.Menus
{
    internal class Menu
    {
        public string Header { get; set; } = "Выберите действие:";

        private readonly List<MenuItem> _items = [];
        private int _selectedIndex = 0;


        public void AddItem(MenuItem item)
        {
            _items.Add(item);
        }

        public int Display()
        {
            ConsoleKey key;
            _selectedIndex = 0;
            do
            {
                Console.Clear();
                Console.WriteLine(Header);
                for (int i = 0; i < _items.Count; i++)
                {
                    if (i == _selectedIndex)
                    {
                        Console.Write("> ");
                    }
                    else
                    {
                        Console.Write("  ");
                    }
                    Console.WriteLine(_items[i].Title);
                }

                key = Console.ReadKey().Key;

                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        _selectedIndex = (_selectedIndex - 1 + _items.Count) % _items.Count;
                        break;
                    case ConsoleKey.DownArrow:
                        _selectedIndex = (_selectedIndex + 1) % _items.Count;
                        break;
                    case ConsoleKey.Enter:
                        _items[_selectedIndex].Command?.Execute();
                        return _selectedIndex;
                }
            } while (true);
        }
    }
}
