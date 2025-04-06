namespace MiniDZ2.Domain.ValueObjects
{
    public record Food
    {
        public string Value { get; }
        public Food(string food)
        {
            if (string.IsNullOrWhiteSpace(food))
            {
                throw new ArgumentException("Пища не может быть пустой");
            }
            Value = food;
        }
    }
}
