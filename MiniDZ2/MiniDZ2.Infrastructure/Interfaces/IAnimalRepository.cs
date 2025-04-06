using MiniDZ2.Domain.Entities;

namespace MiniDZ2.Infrastructure.Interfaces
{
    public interface IAnimalRepository
    {
        public Task<Animal?> GetByIdAsync(Guid id);
        public Task<IEnumerable<Animal>> GetAllAsync();
        public Task<bool> AddAsync(Animal animal);
        public Task RemoveAsync(Guid id);
    }
}
