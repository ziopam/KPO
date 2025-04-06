namespace MiniDZ2.Domain.Events
{
    internal class FeedingTimeEvent(Guid animalId, DateTime time)
    {
        public Guid AnimalId { get; } = animalId;
        public DateTime FeedingTime { get; } = time;
    }
}
