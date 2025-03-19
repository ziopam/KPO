using KR1.Console_Interface.Menus;
using KR1.Database;

public class Program
{
    public static void Main()
    {
        Console.WriteLine("Добро пожаловать в систему учета финансов!");

        FinanceStorageProxy financeStorage = new();

        MainMenu mainMenu = new(financeStorage);
        mainMenu.Display();
        Console.WriteLine("До свидания!");
    }
}

