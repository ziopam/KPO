namespace MiniDZ2.Domain.ValueObjects
{
    /// <summary>
    /// ValueObject для целого положительного числа, которое не может быть равно 0.
    /// </summary>
    public record NoZeroPositiveInt
    {
        /// <summary>
        /// Значение целого положительного числа, которое не может быть равно 0.
        /// </summary>
        /// <example>1</example>
        public int Value { get; }

        /// <summary>
        /// Создает новое целое положительное число, которое не может быть равно 0.
        /// </summary>
        /// <param name="value">Значение</param>
        /// <exception cref="ArgumentException">Происходит, если пользователь передал значение меньше или равно 0.</exception>
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
