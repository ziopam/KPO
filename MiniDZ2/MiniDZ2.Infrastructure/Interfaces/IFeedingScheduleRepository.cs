using MiniDZ2.Domain.Entities;

namespace MiniDZ2.Infrastructure.Interfaces
{
    public interface IFeedingScheduleRepository
    {
        Task<IEnumerable<FeedingSchedule>> GetAllAsync();
        Task<FeedingSchedule?> GetByIdAsync(Guid id);
        Task<bool> AddAsync(FeedingSchedule feedingSchedule);
        Task RemoveAsync(Guid id);

        Task<IEnumerable<FeedingSchedule>> GetByAnimalIdAsync(Guid animalId);
        Task<IEnumerable<FeedingSchedule>> GetByDate(DateOnly date);
    }
}
