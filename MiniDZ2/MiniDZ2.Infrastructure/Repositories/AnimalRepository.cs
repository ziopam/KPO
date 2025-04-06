using MiniDZ2.Domain.Entities;
using MiniDZ2.Infrastructure.Interfaces;
using System.Collections.Concurrent;

namespace MiniDZ2.Infrastructure.Repositories
{
    public class AnimalRepository : IAnimalRepository
    {
        private readonly ConcurrentDictionary<Guid, Animal> _animals = new();

        public Task<bool> AddAsync(Animal animal)
        {
            return Task.FromResult(_animals.TryAdd(animal.Id, animal));
        }

        public Task<IEnumerable<Animal>> GetAllAsync()
        {
            return Task.FromResult<IEnumerable<Animal>>(_animals.Values);
        }

        public Task<Animal?> GetByIdAsync(Guid id)
        {
            _animals.TryGetValue(id, out var animal);
            return Task.FromResult(animal);
        }

        public Task RemoveAsync(Guid id)
        {
            _animals.TryRemove(id, out _);
            return Task.CompletedTask;
        }
    }
}
