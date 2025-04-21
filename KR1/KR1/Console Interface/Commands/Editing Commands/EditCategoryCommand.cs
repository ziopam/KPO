using KR1.Database;
using KR1.DomainObjects;
using System.Text;

namespace KR1.Console_Interface.Commands.Editing_Commands
{
    internal class EditCategoryCommand(IFinanceDatabase financeDatabase) : ICommand
    {
        private readonly IFinanceDatabase financeDatabase = financeDatabase;

        public void Execute()
        {
            var categories = financeDatabase.GetCategories();

            if (!categories.Any())
            {
                Console.WriteLine("Нет данных для редактирования");
                return;
            }

            Category category = DataGetterFacade.GetCategoryFromUser("Выберите категорию для редактирования: ", categories);
            int selectedField = DataGetterFacade.GetOptionsFromUser("Выберите поле для редактирования: ", ["Название", "Тип"]);

            StringBuilder sb = new();
            if (selectedField == 0)
            {
                string newName = DataGetterFacade.GetStringFromUser("Введите новое название категории: ");
                sb.AppendLine($"ID: {category.Id}");
                sb.AppendLine($"Название: {category.Name} -> {newName}");
                sb.AppendLine($"Тип: {(category.IsIncome ? "Доход" : "Расход")}");
                sb.Append("Вы уверены, что хотите изменить название этой категории?");
                if (DataGetterFacade.GetBoolFromUser(sb.ToString()))
                {
                    financeDatabase.UpdateCategoryName(category.Id, newName);
                    Console.WriteLine("Категория успешно изменена");
                    return;
                }
            }
            else
            {
                sb.AppendLine($"ID: {category.Id}");
                sb.AppendLine($"Название: {category.Name}");
                sb.AppendLine($"Тип: {(category.IsIncome ? "Доход" : "Расход")} -> {(!category.IsIncome ? "Доход" : "Расход")}");
                sb.Append("Вы уверены, что хотите изменить тип этой категории?");
                if (DataGetterFacade.GetBoolFromUser(sb.ToString()))
                {
                    financeDatabase.UpdateCategoryIsIncome(category.Id, !category.IsIncome);
                    Console.WriteLine("Категория успешно изменена");
                    return;
                }
            }

            Console.WriteLine("Отмена изменения категории");
        }
    }
}
