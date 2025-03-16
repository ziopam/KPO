using KR1.Console_Interface;
using KR1.Database;

// Консольный интерфейс
public class Program
{
    public static void Main()
    {
        Console.WriteLine("Добро пожаловать в систему учета финансов!");

        FinanceStorage financeStorage = new();

        MainMenu mainMenu = new(financeStorage);
        mainMenu.Display();
        Console.WriteLine("До свидания!");
    }
}

