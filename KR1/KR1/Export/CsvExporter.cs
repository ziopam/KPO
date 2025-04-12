using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.Text;

namespace KR1.Export
{
    internal class CsvExporter<T> : DataExporterTemplate<T>
    {
        private readonly static CsvConfiguration config = new(CultureInfo.InvariantCulture) { Delimiter = ";", Encoding = Encoding.UTF8 };

        public CsvExporter()
        {
            FilePath += ".csv";
        }

        protected override string SerializeData(List<T> data)
        {
            try
            {
                using var writer = new StringWriter();
                using var csv = new CsvWriter(writer, config);
                csv.WriteRecords(data);
                return writer.ToString();
            }
            catch (Exception)
            {
                Console.WriteLine("Ошибка при сериализации данных");
                Environment.Exit(1);
                return null;
            }
        }
    }
}
