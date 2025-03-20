using KR1.Console_Interface.Menus;
using KR1.Database;
using Microsoft.Extensions.DependencyInjection;


public class Program
{
    public static void Main()
    {
        var serviceProvider = new ServiceCollection()
            .AddSingleton<FinanceDatabase>()
            .AddSingleton<IFinanceDatabase, FinanceStorageProxy>()
            .AddSingleton<MainMenu>()
            .BuildServiceProvider();


        MainMenu mainMenu = serviceProvider.GetRequiredService<MainMenu>();
        mainMenu.Display();
    }
}

