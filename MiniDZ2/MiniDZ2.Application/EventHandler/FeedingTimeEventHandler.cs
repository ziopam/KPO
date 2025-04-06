using MediatR;
using MiniDZ2.Domain.Events;

namespace MiniDZ2.Application.EventHandler
{
    public class FeedingTimeEventHandler : INotificationHandler<FeedingTimeEvent>
    {
        public Task Handle(FeedingTimeEvent notification, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                Console.WriteLine($"Время кормления для животного с ID {notification.AnimalId} было изменено на {notification.FeedingTime:dd-MM-yyyy}");
            }, cancellationToken);
        }
    }
}
