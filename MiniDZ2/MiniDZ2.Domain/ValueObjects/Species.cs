namespace MiniDZ2.Domain.ValueObjects
{
    /// <summary>
    /// ValueObject для вида животного.
    /// </summary>
    public record Species
    {
        private static readonly List<string> species = new()
        {
            "Хищник",
            "Травоядное",
            "Рыба",
            "Птица",
        };

        public static readonly Species Predator = new(species[0]);
        public static readonly Species Herbivore = new(species[1]);
        public static readonly Species Fish = new(species[2]);
        public static readonly Species Bird = new(species[3]);

        /// <summary>
        /// Вид животного. Может быть только из списка: [Хищник, Травоядное, Рыба, Птица].
        /// </summary>
        /// <example>Рыба</example>
        public string Value { get; }

        private Species(string value) => Value = value;

        public static Species GetSpeciesByString(string value)
        {
            return value switch
            {
                "Хищник" => Predator,
                "Травоядное" => Herbivore,
                "Рыба" => Fish,
                "Птица" => Bird,
                _ => throw new ArgumentException($"Вид животного может быть только из этого списка: {string.Join(", ", species)}")
            };
        }
    }
}
