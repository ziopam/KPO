namespace MiniDZ2.Domain.ValueObjects
{
    public record NoZeroPositiveInt
    {
        public int Value { get; }

        public NoZeroPositiveInt(int value)
        {
            if (value <= 0)
            {
                throw new ArgumentException("Значение не может быть или равно 0");
            }
            Value = value;
        }
    }
}
