using MediatR;

namespace MiniDZ2.Domain.Events
{
    public class FeedingTimeEvent(Guid animalId, DateTime time) : INotification
    {
        public Guid AnimalId { get; } = animalId;
        public DateTime FeedingTime { get; } = time;
    }
}
