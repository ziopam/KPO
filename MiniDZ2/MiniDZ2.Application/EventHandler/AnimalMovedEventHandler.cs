using MediatR;
using MiniDZ2.Domain.Events;

namespace MiniDZ2.Application.EventHandler
{
    public class AnimalMovedEventHandler : INotificationHandler<AnimalMovedEvent>
    {
        public Task Handle(AnimalMovedEvent notification, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                Console.WriteLine($"Животное с ID {notification.AnimalId} было перемещено из вольера с ID {notification.FromEnclosureId} в {notification.ToEnclosureId}");
            }, cancellationToken);
        }
    }
}
