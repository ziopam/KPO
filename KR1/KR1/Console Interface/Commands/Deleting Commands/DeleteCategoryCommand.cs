using KR1.Database;
using KR1.DomainObjects;
using System.Text;

namespace KR1.Console_Interface.Commands.Deleting_Commands
{
    internal class DeleteCategoryCommand(IFinanceDatabase financeDatabase) : ICommand
    {
        private readonly IFinanceDatabase financeDatabase = financeDatabase;

        public void Execute()
        {
            var categories = financeDatabase.GetCategories();

            if (!categories.Any())
            {
                Console.WriteLine("Нет данных для удаления");
                return;
            }

            Category selectedCategory = DataGetterFacade.GetCategoryFromUser("Выберите категорию для удаления: ", categories);

            StringBuilder sb = new();
            sb.AppendLine($"ID: {selectedCategory.Id}");
            sb.AppendLine($"Название категории: {selectedCategory.Name}");
            sb.AppendLine($"Тип категории: {(selectedCategory.IsIncome ? "Доход" : "Расход")}");
            sb.Append("Вы уверены, что хотите удалить эту категорию?");

            if (DataGetterFacade.GetBoolFromUser(sb.ToString()))
            {
                financeDatabase.RemoveCategory(selectedCategory.Id);
                Console.WriteLine("Категория успешно удалена");
                return;
            }

            Console.WriteLine("Отмена удаления категории");
        }
    }
}
