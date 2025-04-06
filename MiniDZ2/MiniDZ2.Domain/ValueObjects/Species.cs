namespace MiniDZ2.Domain.ValueObjects
{
    public record Species
    {
        public string Value { get; }
        public Species(string species)
        {
            if (string.IsNullOrWhiteSpace(species))
            {
                throw new ArgumentException("Вид не может быть пустым");
            }
            Value = species;
        }
    }
}
