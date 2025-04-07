namespace MiniDZ2.Domain.ValueObjects
{
    /// <summary>
    /// ValueObject для типа вольера.
    /// </summary>
    public record EnclosureType
    {
        private static readonly List<string> EnclosureTypes =
            [
                "Для хищников",
                "Для травоядных",
                "Для птиц",
                "Для рыб"
            ];
        /// <summary>
        /// Тип вольера для хищников.
        /// </summary>
        public static readonly EnclosureType forPredators = new(EnclosureTypes[0]);

        /// <summary>
        /// Тип вольера для травоядных.
        /// </summary>
        public static readonly EnclosureType forHerbivores = new(EnclosureTypes[1]);

        /// <summary>
        /// Тип вольера для птиц.
        /// </summary>
        public static readonly EnclosureType forBirds = new(EnclosureTypes[2]);

        /// <summary>
        /// Тип вольера для рыб.
        /// </summary>
        public static readonly EnclosureType forFish = new(EnclosureTypes[3]);

        /// <summary>
        /// Тип вольера.
        /// </summary>
        /// <example>Для хищников</example>
        public string Value { get; }

        private EnclosureType(string value) => Value = value;

        /// <summary>
        /// Возвращает тип вольера из строки.
        /// </summary>
        /// <param name="value">Тип вольера.</param>
        /// <returns>EnclosureType.</returns>
        /// <exception cref="ArgumentException">Происходит, если пользователь привел неизвесный тип вольера.</exception>
        public static EnclosureType GetFromString(string value)
        {
            return value switch
            {
                "Для хищников" => forPredators,
                "Для травоядных" => forHerbivores,
                "Для птиц" => forBirds,
                "Для рыб" => forFish,
                _ => throw new ArgumentException($"Тип вольера должен быть из этого списка: {string.Join(", ", EnclosureTypes)}"),
            };
        }
    }
}
