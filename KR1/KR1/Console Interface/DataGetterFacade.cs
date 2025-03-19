using System.Globalization;

namespace KR1.Console_Interface
{
    using KR1.Console_Interface.Menus;
    using KR1.DomainObjects;

    internal static class DataGetterFacade
    {
        public static String GetStringFromUser(string prompt, bool canBeEmpty = false)
        {
            Console.Clear();
            Console.Write(prompt);

            while (true)
            {
                String? result = Console.ReadLine();
                if (String.IsNullOrWhiteSpace(result) && !canBeEmpty)
                {
                    Console.WriteLine("Пустая строка недопустима. Повторите ввод.");
                }
                else
                {
                    return !String.IsNullOrEmpty(result) ? result : "";
                }
            }
        }

        public static decimal GetDecimalFromUser(string prompt, bool isPostitiveOnly = false)
        {
            Console.Clear();
            Console.Write(prompt);
            while (true)
            {
                String? input = Console.ReadLine();
                if (Decimal.TryParse(input, out decimal result))
                {
                    if (isPostitiveOnly && result <= 0)
                    {
                        Console.WriteLine("Число должно быть положительным. Повторите ввод.");
                        continue;
                    }

                    return result;
                }
                else
                {
                    Console.WriteLine("Неверный формат числа. Повторите ввод.");
                }
            }
        }

        public static int GetOptionsFromUser(string prompt, List<string> options)
        {
            Console.Clear();
            Menu chooseMenu = new()
            {
                Header = prompt
            };

            foreach (string option in options)
            {
                chooseMenu.AddItem(new MenuItem { Title = option });
            }

            int selectedIndex = chooseMenu.Display();
            return selectedIndex;
        }

        public static bool GetBoolFromUser(string prompt)
        {
            List<string> options = ["Да", "Нет"];
            int selectedIndex = GetOptionsFromUser(prompt, options);
            return selectedIndex == 0;
        }

        public static BankAccount GetBankAccountFromUser(string prompt, IEnumerable<BankAccount> accounts)
        {
            int selectedAccountId = DataGetterFacade.GetOptionsFromUser(prompt, accounts.Select(account => account.Name).ToList());
            return accounts.ElementAt(selectedAccountId);
        }

        public static Category GetCategoryFromUser(string prompt, IEnumerable<Category> categories)
        {
            int selectedCategoryId = DataGetterFacade.GetOptionsFromUser(prompt, categories.Select(category => category.Name).ToList());
            return categories.ElementAt(selectedCategoryId);
        }

        public static Operation GetOperationFromUser(string prompt, IEnumerable<Operation> operations)
        {
            int selectedOperationId = DataGetterFacade.GetOptionsFromUser(prompt, operations.Select(operation => operation.Id.ToString()).ToList());
            return operations.ElementAt(selectedOperationId);
        }

        public static DateOnly GetDateFromUser(string prompt)
        {
            Console.Clear();
            Console.WriteLine(prompt);

            String format = "dd-MM-yyyy";
            Console.Write($"Введите дату в формате {format}: ");

            while (true)
            {
                String? input = Console.ReadLine();
                if (DateOnly.TryParseExact(input, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly result))
                {
                    return result;
                }
                else
                {
                    Console.WriteLine("Неверный формат даты. Повторите ввод.");
                }
            }
        }
    }
}
