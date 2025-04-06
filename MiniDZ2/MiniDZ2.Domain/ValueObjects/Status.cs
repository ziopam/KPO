namespace MiniDZ2.Domain.ValueObjects
{
    public record Status
    {
        public static readonly Status Healthy = new("Здоров");
        public static readonly Status Sick = new("Болен");

        public string Value { get; }

        private Status(string value) => Value = value;
    }
}
