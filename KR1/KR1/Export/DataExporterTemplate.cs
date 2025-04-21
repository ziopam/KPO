using System.Text;

internal abstract class DataExporterTemplate<T>
{
    public string FilePath { get; set; } = $"{typeof(T).Name}";

    public void Export(List<T> data)
    {
        string exportedData = SerializeData(data);
        SaveToFile(exportedData);
    }

    protected abstract string SerializeData(List<T> data);

    protected virtual void SaveToFile(string data)
    {
        try
        {
            System.IO.File.WriteAllText(FilePath, data, Encoding.UTF8);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Ошибка при сохранении данных в файл: {e.Message}");
            Environment.Exit(1);
        }
    }
}