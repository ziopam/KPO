using MiniDZ2.Domain.Entities;
using MiniDZ2.Infrastructure.Interfaces;
using System.Collections.Concurrent;

namespace MiniDZ2.Infrastructure.Repositories
{
    public class FeedingScheduleRepository : IFeedingScheduleRepository
    {
        private readonly ConcurrentDictionary<Guid, FeedingSchedule> _feedingSchedules = new();

        public Task<bool> AddAsync(FeedingSchedule feedingSchedule)
        {
            return Task.FromResult(_feedingSchedules.TryAdd(feedingSchedule.Id, feedingSchedule));
        }

        public Task<IEnumerable<FeedingSchedule>> GetAllAsync()
        {
            return Task.FromResult<IEnumerable<FeedingSchedule>>(_feedingSchedules.Values);
        }

        public Task<IEnumerable<FeedingSchedule>> GetByAnimalIdAsync(Guid animalId)
        {
            var schedules = _feedingSchedules.Values
                .Where(fs => fs.AnimalId == animalId)
                .ToList();
            return Task.FromResult<IEnumerable<FeedingSchedule>>(schedules);
        }

        public Task<IEnumerable<FeedingSchedule>> GetByEnclosureIdAsync(Guid enclosureId)
        {
            throw new NotImplementedException();
        }

        public Task<FeedingSchedule?> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task RemoveAsync(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
