using System.Text.Json;

namespace KR1.Export
{
    internal class JsonExporter<T> : DataExporterTemplate<T>
    {
        private readonly static JsonSerializerOptions options = new() { Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping, WriteIndented = true };

        public JsonExporter()
        {
            FilePath += ".json";
        }

        protected override string SerializeData(List<T> data)
        {
            try
            {
                return JsonSerializer.Serialize(data, options);
            }
            catch (NotSupportedException)
            {
                Console.WriteLine("Ошибка при сериализации данных");
                Environment.Exit(1);
                return null;
            }
        }
    }
}
