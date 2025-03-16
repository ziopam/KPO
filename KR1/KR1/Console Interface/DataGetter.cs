namespace KR1.Console_Interface
{
    internal static class DataGetter
    {
        public static String GetStringFromUser(string prompt)
        {
            Console.Write(prompt);

            while (true)
            {
                String? result = Console.ReadLine();
                if (String.IsNullOrWhiteSpace(result))
                {
                    Console.WriteLine("Пустая строка недопустима. Повторите ввод.");
                }
                else
                {
                    return result;
                }
            }
        }

        public static decimal GetDecimalFromUser(string prompt)
        {
            Console.Write(prompt);
            while (true)
            {
                String? input = Console.ReadLine();
                if (Decimal.TryParse(input, out decimal result))
                {
                    return result;
                }
                else
                {
                    Console.WriteLine("Неверный формат числа. Повторите ввод.");
                }
            }
        }
    }
}
