using MiniDZ2.Domain.ValueObjects;

namespace MiniDZ2.Domain.Entities
{
    public class FeedingSchedule(Guid animalId, DateTime feedingTime, Food food)
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public Guid AnimalId { get; private set; } = animalId;
        public DateTime FeedingTime { get; private set; } = feedingTime;
        public Food Food { get; private set; } = food;

        public void UpdateTime(DateTime newTime) => FeedingTime = newTime;
        public void MarkAsCompleted() { /* ... */ }
    }
}
