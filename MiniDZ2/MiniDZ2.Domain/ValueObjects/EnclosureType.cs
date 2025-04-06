namespace MiniDZ2.Domain.ValueObjects
{
    public record EnclosureType
    {
        public static readonly EnclosureType forPredators = new("Для хищников");
        public static readonly EnclosureType forHerbivores = new("Для травоядных");
        public static readonly EnclosureType forBirds = new("Для птиц");
        public static readonly EnclosureType forFish = new("Для рыб");

        public string Value { get; }

        private EnclosureType(string value) => Value = value;
    }
}
