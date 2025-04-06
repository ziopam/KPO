using MiniDZ2.Domain.Entities;
using MiniDZ2.Infrastructure.Interfaces;
using System.Collections.Concurrent;

namespace MiniDZ2.Infrastructure.Repositories
{
    public class EnclosureRepository : IEnclosureRepository
    {
        private readonly ConcurrentDictionary<Guid, Enclosure> _enclosures = new();

        public Task<bool> AddAsync(Enclosure enclosure)
        {
            return Task.FromResult(_enclosures.TryAdd(enclosure.Id, enclosure));
        }

        public Task<IEnumerable<Enclosure>> GetAllAsync()
        {
            return Task.FromResult<IEnumerable<Enclosure>>(_enclosures.Values);
        }

        public Task<Enclosure?> GetByIdAsync(Guid id)
        {
            _enclosures.TryGetValue(id, out var enclosure);
            return Task.FromResult(enclosure);
        }

        public Task<Enclosure?> GetEnclosureByAnimalIdAsync(Guid animalId)
        {
            foreach (var enclosure in _enclosures.Values)
            {
                if (enclosure.AnimalIds.Any(id => id == animalId))
                {
                    return Task.FromResult<Enclosure?>(enclosure);
                }
            }
            return Task.FromResult<Enclosure?>(null);
        }

        public Task RemoveAsync(Guid id)
        {
            _enclosures.TryRemove(id, out _);
            return Task.CompletedTask;
        }
    }
}
