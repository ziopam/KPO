using MediatR;

namespace MiniDZ2.Domain.Events
{
    public class AnimalMovedEvent(Guid animalId, Guid from, Guid to) : INotification
    {
        public Guid AnimalId { get; } = animalId;
        public Guid FromEnclosureId { get; } = from;
        public Guid ToEnclosureId { get; } = to;
    }
}
