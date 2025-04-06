namespace MiniDZ2.Domain.ValueObjects
{
    internal record Food
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
