using MiniDZ2.Domain.Entities;

namespace MiniDZ2.Infrastructure.Interfaces
{
    public interface IEnclosureRepository
    {
        public Task<Enclosure?> GetByIdAsync(Guid id);
        public Task<IEnumerable<Enclosure>> GetAllAsync();
        public Task<bool> AddAsync(Enclosure enclosure);
        public Task RemoveAsync(Guid id);

        public Task<Enclosure?> GetEnclosureByAnimalIdAsync(Guid animalId);
    }
}
