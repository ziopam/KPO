namespace MiniDZ2.Domain.ValueObjects
{
    /// <summary>
    /// ValueObject для пола животного.
    /// </summary>
    public record Gender
    {
        private static readonly List<string> genders = [
            "Самец",
            "Самка",
        ];

        /// <summary>
        /// Пол животного - самец.
        /// </summary>
        public static readonly Gender Male = new(genders[0]);

        /// <summary>
        /// Пол животного - самка.
        /// </summary>
        public static readonly Gender Female = new(genders[1]);

        /// <summary>
        /// Пол животного.
        /// </summary>
        /// <example>Самец</example>
        public string Value { get; }

        private Gender(string value) => Value = value;

        /// <summary>
        /// Возвращает пол животного из строки.
        /// </summary>
        /// <param name="value">Пол животного.</param>
        /// <returns>Gender</returns>
        /// <exception cref="ArgumentException">Происходит, если пользователь передал неизвестный пол животного.</exception>
        public static Gender GetGenderByString(string value)
        {
            return value switch
            {
                "Самец" => Gender.Male,
                "Самка" => Gender.Female,
                _ => throw new ArgumentException($"Пол животного может быть только из этого списка: {string.Join(", ", genders)}")
            };
        }
    }
}
