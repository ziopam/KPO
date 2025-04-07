namespace MiniDZ2.Domain.ValueObjects
{
    /// <summary>
    /// ValueObject для статуса животного.
    /// </summary>
    public record Status
    {
        private static readonly List<string> statuses = new()
        {
            "Здоров",
            "Болен"
        };

        /// <summary>
        /// Статус животного - здоров.
        /// </summary>
        public static readonly Status Healthy = new(statuses[0]);

        /// <summary>
        /// Статус животного - болен.
        /// </summary>
        public static readonly Status Sick = new(statuses[1]);

        /// <summary>
        /// Статус животного.
        /// </summary>
        /// <example>Здоров</example>
        public string Value { get; }

        private Status(string value) => Value = value;

        /// <summary>
        /// Возвращает статус животного из строки.
        /// </summary>
        /// <param name="value">Статус животного.</param>
        /// <returns>Statis.</returns>
        /// <exception cref="ArgumentException">Происходит, если пользователь передал незнакомый тип.</exception>
        public static Status GetStatusByString(string value)
        {
            return value switch
            {
                "Здоров" => Status.Healthy,
                "Болен" => Status.Sick,
                _ => throw new ArgumentException($"Статус животного может быть только из этого списка: {string.Join(", ", statuses)}")
            };
        }
    }
}
