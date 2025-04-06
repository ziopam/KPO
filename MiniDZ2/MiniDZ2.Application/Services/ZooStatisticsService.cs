using MiniDZ2.Domain.ValueObjects;
using MiniDZ2.Infrastructure.Interfaces;

namespace MiniDZ2.Application.Services
{
    public class ZooStatisticsService(IAnimalRepository animalRepo, IEnclosureRepository enclosureRepo, IFeedingScheduleRepository feedingScheduleRepo)
    {
        private readonly IAnimalRepository _animalRepo = animalRepo;
        private readonly IEnclosureRepository _enclosureRepo = enclosureRepo;
        private readonly IFeedingScheduleRepository _feedingScheduleRepo = feedingScheduleRepo;

        public async Task<int> GetTotalAnimalCountAsync()
        {
            var animals = await _animalRepo.GetAllAsync();
            return animals.Count();
        }

        public async Task<int> GetTotalHealthyAnimalsAsync()
        {
            var animals = await _animalRepo.GetAllAsync();
            return animals.Count(a => a.Status == Status.Healthy);
        }

        public async Task<int> GetTotalSickAnimalsAsync()
        {
            var animals = await _animalRepo.GetAllAsync();
            return animals.Count(a => a.Status == Status.Sick);
        }

        public async Task<int> GetTotalEnclosureCountAsync()
        {
            var enclosures = await _enclosureRepo.GetAllAsync();
            return enclosures.Count();
        }

        public async Task<int> GetAvailableEnclosuresAsync()
        {
            var enclosures = await _enclosureRepo.GetAllAsync();
            return enclosures.Count(e => e.HasAvaluablePlace());
        }

        public async Task<int> GetTotalFeedingScheduleCountAsync()
        {
            var feedingSchedules = await _feedingScheduleRepo.GetAllAsync();
            return feedingSchedules.Count();
        }
    }
}
