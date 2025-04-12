using KR1.Database;
using KR1.DomainObjects;
using System.Text;

namespace KR1.Console_Interface.Commands.Deleting_Commands
{
    internal class DeleteOperationCommand(IFinanceDatabase financeDatabase) : ICommand
    {
        private readonly IFinanceDatabase financeDatabase = financeDatabase;

        public void Execute()
        {
            var operations = financeDatabase.GetOperations();

            if (!operations.Any())
            {
                Console.WriteLine("Нет данных для удаления");
                return;
            }

            Operation operation = DataGetterFacade.GetOperationFromUser("Выберите операцию для удаления: ", operations);

            StringBuilder sb = new();
            sb.AppendLine($"ID: {operation.Id}");
            sb.AppendLine($"ID счета: {operation.BankAccountId}");
            sb.AppendLine($"ID категории: {operation.CategoryId}");
            sb.AppendLine($"Сумма: {operation.Amount}");
            sb.AppendLine($"Дата: {operation.Date}");
            sb.AppendLine($"Тип операции: {(operation.IsIncome ? "Доход" : "Расход")}");
            sb.AppendLine($"Описание: {operation.Description}");
            sb.Append("Вы уверены, что хотите удалить эту операцию?");


            if (DataGetterFacade.GetBoolFromUser(sb.ToString()))
            {
                financeDatabase.RemoveOperation(operation.Id);
                Console.WriteLine("Операция успешно удалена");
                return;
            }

            Console.WriteLine("Отмена удаления операции");
        }
    }
}
