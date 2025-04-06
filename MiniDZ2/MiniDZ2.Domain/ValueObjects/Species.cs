namespace MiniDZ2.Domain.ValueObjects
{
    internal record Species
    {
        public string Value { get; }
        public bool IsDangerous { get; }
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
