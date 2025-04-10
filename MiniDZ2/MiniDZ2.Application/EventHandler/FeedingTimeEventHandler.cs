using MediatR;
using MiniDZ2.Application.Interfaces;
using MiniDZ2.Domain.Events;

namespace MiniDZ2.Application.EventHandler
{
    public class FeedingTimeEventHandler(IFeedingOrganizationService feedingOrganizationService) : INotificationHandler<FeedingTimeEvent>
    {
        private readonly IFeedingOrganizationService _feedingOrganizationService = feedingOrganizationService;

        public Task Handle(FeedingTimeEvent notification, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                Console.WriteLine($"Настало время кормления для животного с ID {notification.AnimalId}, согласно расписанию с ID {notification.ScheduleId}");
                _feedingOrganizationService.FeedAnimal(notification.AnimalId, notification.ScheduleId);
            }, cancellationToken);
        }
    }
}
