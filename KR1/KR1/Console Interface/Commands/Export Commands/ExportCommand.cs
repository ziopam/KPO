using KR1.Export;

namespace KR1.Console_Interface.Commands.Export_Commands
{
    internal class ExportCommand<T>(List<T> data) : ICommand
    {
        private CsvExporter<T> csvExporter = new();
        private JsonExporter<T> jsonExporter = new();
        private DataExporterTemplate<T>? _exporter;
        private readonly List<T> _data = data;

        public void Execute()
        {
            int selectedOption = DataGetterFacade.GetOptionsFromUser("Выберите формат экспорта: ", ["CSV", "JSON"]);
            switch (selectedOption)
            {
                case 0:
                    _exporter = csvExporter;
                    break;
                case 1:
                    _exporter = jsonExporter;
                    break;
            }

            if (DataGetterFacade.GetBoolFromUser($"Вы уверены, что хотите экспортировать {typeof(T).Name}?"))
            {
                _exporter!.Export(_data);
                Console.WriteLine($"Файл {_exporter!.FilePath} успешно сохранен рядом с исполняемым файлом.");
                return;
            }
            Console.WriteLine("Отмена экспорта");
        }
    }
}
