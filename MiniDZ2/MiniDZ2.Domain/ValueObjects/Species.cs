namespace MiniDZ2.Domain.ValueObjects
{
    /// <summary>
    /// ValueObject для вида животного.
    /// </summary>
    public record Species
    {
        /// <summary>
        /// Вид животного.
        /// </summary>
        /// <example>Лев</example>
        public string Value { get; }

        /// <summary>
        /// Опасен ли вид животного.
        /// </summary>
        /// <example>true</example>
        public bool IsDangerous { get; }

        /// <summary>
        /// Создает новый вид животного.
        /// </summary>
        /// <param name="species">Вид животного.</param>
        /// <param name="isDangerous">Опасно ли животное.</param>
        /// <exception cref="ArgumentException">Происходит, если вид животного пустое.</exception>
        public Species(string species, bool isDangerous)
        {
            if (string.IsNullOrWhiteSpace(species))
            {
                throw new ArgumentException("Вид не может быть пустым");
            }
            Value = species;
            IsDangerous = isDangerous;
        }
    }
}
