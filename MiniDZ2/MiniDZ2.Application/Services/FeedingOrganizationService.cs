using MediatR;
using MiniDZ2.Domain.Entities;
using MiniDZ2.Domain.Events;
using MiniDZ2.Domain.ValueObjects;
using MiniDZ2.Infrastructure.Interfaces;

namespace MiniDZ2.Application.Services
{
    public class FeedingOrganizationService(IFeedingScheduleRepository feedingScheduleRepo, IAnimalRepository animalRepo, IMediator mediator)
    {
        private readonly IFeedingScheduleRepository _feedingScheduleRepo = feedingScheduleRepo;
        private readonly IAnimalRepository _animalRepo = animalRepo;
        private readonly IMediator _mediator = mediator;

        public async Task AddFeedingScheduleAsync(Guid animalId, DateTime feedingTime, Food foodType)
        {
            var _ = await _animalRepo.GetByIdAsync(animalId) ?? throw new ArgumentException("Животное не найдено");

            var feedingSchedule = new FeedingSchedule(animalId, feedingTime, foodType);
            await _feedingScheduleRepo.AddAsync(feedingSchedule);

            var @event = new FeedingTimeEvent(animalId, feedingTime);
            await _mediator.Publish(@event);
        }

        public async Task<IEnumerable<FeedingSchedule>> GetFeedingSchedulesForAnimalAsync(Guid animalId)
        {
            return await _feedingScheduleRepo.GetByAnimalIdAsync(animalId);
        }

        public async Task ChangeFeedingTimeAsync(Guid scheduleId, DateTime newFeedingTime)
        {
            var feedingSchedule = await _feedingScheduleRepo.GetByIdAsync(scheduleId)
                ?? throw new ArgumentException("Расписание не найдено");
            feedingSchedule.UpdateTime(newFeedingTime);
        }
    }
}
