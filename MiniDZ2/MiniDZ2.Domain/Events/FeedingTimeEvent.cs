using MediatR;

namespace MiniDZ2.Domain.Events
{
    public class FeedingTimeEvent(Guid animalId, Guid scheduleId) : INotification
    {
        public Guid AnimalId { get; } = animalId;
        public Guid ScheduleId { get; } = scheduleId;
    }
}
