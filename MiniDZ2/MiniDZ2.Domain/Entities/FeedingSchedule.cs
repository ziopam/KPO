using MiniDZ2.Domain.ValueObjects;

namespace MiniDZ2.Domain.Entities
{
    public class FeedingSchedule(Guid animalId, DateOnly feedingTime, Food food)
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public Guid AnimalId { get; private set; } = animalId;
        public DateOnly FeedingTime { get; private set; } = feedingTime;
        public Food Food { get; private set; } = food;
        public bool IsCompleted { get; private set; } = false;

        public void UpdateTime(DateOnly newTime) => FeedingTime = newTime;

        public void MarkAsCompleted()
        {
            IsCompleted = true;
        }

        public void MarkAsNotCompleted()
        {
            IsCompleted = false;
        }
    }
}
