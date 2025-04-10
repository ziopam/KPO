using MiniDZ2.Application.Interfaces;
using MiniDZ2.Infrastructure.Interfaces;

namespace MiniDZ2.Application.Services
{
    public class FeedingOrganizationService(IFeedingScheduleRepository feedingScheduleRepo, IAnimalRepository animalRepo) : IFeedingOrganizationService
    {
        private readonly IFeedingScheduleRepository _feedingScheduleRepo = feedingScheduleRepo;
        private readonly IAnimalRepository _animalRepo = animalRepo;

        public async void FeedAnimal(Guid animalId, Guid scheduleId)
        {
            var animal = await _animalRepo.GetByIdAsync(animalId);
            var schedule = await _feedingScheduleRepo.GetByIdAsync(scheduleId);

            if (animal == null)
            {
                throw new ArgumentException($"Животного с ID {animalId} не найдено.");
            }
            if (schedule == null)
            {
                throw new ArgumentException($"Расписания с ID {scheduleId} не найдено.");
            }

            animal.Feed();
            schedule.MarkAsCompleted();
        }
    }
}
