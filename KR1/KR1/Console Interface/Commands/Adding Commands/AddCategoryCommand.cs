using KR1.Database;
using System.Text;

namespace KR1.Console_Interface.Commands
{
    internal class AddCategoryCommand(IFinanceDatabase financeDatabase) : ICommand
    {

        private readonly IFinanceDatabase financeDatabase = financeDatabase;

        public void Execute()
        {
            string name = DataGetterFacade.GetStringFromUser("Введите название категории: ");
            bool isIncome = DataGetterFacade.GetOptionsFromUser("Новая категория это расход или доход? ", ["Доход", "Расход"]) == 0;
            Console.Clear();

            StringBuilder sb = new();
            sb.AppendLine($"Название категории: {name}");
            sb.AppendLine($"Вид категории: {(isIncome ? "Доход" : "Расход")}");
            sb.Append("Вы уверены, что хотите добавить эту категорию?");

            if (DataGetterFacade.GetBoolFromUser(sb.ToString()))
            {
                financeDatabase.AddCategory(name, isIncome);
                Console.WriteLine("Категория успешно добавлена");
                return;
            }

            Console.WriteLine("Отмена добавления категории");
        }
    }
}
