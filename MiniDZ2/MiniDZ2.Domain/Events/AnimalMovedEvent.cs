namespace MiniDZ2.Domain.Events
{
    internal class AnimalMovedEvent(Guid animalId, Guid from, Guid to)
    {
        public Guid AnimalId { get; } = animalId;
        public Guid FromEnclosureId { get; } = from;
        public Guid ToEnclosureId { get; } = to;
    }
}
