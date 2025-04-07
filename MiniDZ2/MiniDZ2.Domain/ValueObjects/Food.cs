namespace MiniDZ2.Domain.ValueObjects
{
    /// <summary>
    /// ValueObject для еды животного.
    /// </summary>
    public record Food
    {
        /// <summary>
        /// Пища животного.
        /// </summary>
        /// <example>Мясо</example>
        public string Value { get; }

        /// <summary>
        /// Создает новую пищу животного.
        /// </summary>
        /// <param name="food">Название еды.</param>
        /// <exception cref="ArgumentException">Падает, если название еды пустое.</exception>
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
