namespace MiniDZ2.Domain.ValueObjects
{
    public record BirthDate
    {
        public DateTime Value { get; }
        public BirthDate(DateTime value)
        {
            if (value > DateTime.Now)
                throw new ArgumentException("День рождения не может быть в будущем.");
            Value = value;
        }

        public override string ToString()
        {
            return Value.ToString("dd-MM-yyyy");
        }
    }
}
