namespace MiniDZ2.Domain.ValueObjects
{
    /// <summary>
    /// ValueObject для даты рождения животного.
    /// </summary>
    public record BirthDate
    {
        /// <summary>
        /// Дата рождения животного.
        /// </summary>
        /// <example>01-01-2020</example>
        public DateOnly Value { get; }

        /// <summary>
        /// Создает новую дату рождения животного.
        /// </summary>
        /// <param name="stringValue">Дата рождения в формате dd-MM-yyyy.</param>
        /// <exception cref="ArgumentException"></exception>
        public BirthDate(string stringValue)
        {
            DateOnly value;
            bool success = DateOnly.TryParse(stringValue, out value);
            if (!success)
            {
                throw new ArgumentException("Некорректная дата рождения. Дата должна быть в формате dd-MM-yyyy.");
            }
            if (value > DateOnly.FromDateTime(DateTime.Today))
            {
                throw new ArgumentException("День рождения не может быть в будущем.");
            }
            Value = value;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return Value.ToString("dd-MM-yyyy");
        }
    }
}
