using MiniDZ2.Application.Interfaces;
using MiniDZ2.Infrastructure.Interfaces;

namespace MiniDZ2.Application.Services
{
    public class RemoveAnimalFromEnclosureService(IAnimalRepository animalRepo, IEnclosureRepository enclosureRepo) : IRemoveAnimalFromEnclosureService
    {
        private readonly IAnimalRepository _animalRepo = animalRepo;
        private readonly IEnclosureRepository _enclosureRepo = enclosureRepo;

        public async Task RemoveAnimalAsync(Guid animalId, Guid enclosureId)
        {
            var animal = await _animalRepo.GetByIdAsync(animalId);
            var enclosure = await _enclosureRepo.GetByIdAsync(enclosureId);
            enclosure?.RemoveAnimal(animal?.Id ?? Guid.Empty);
            animal?.MoveToEnclosure(Guid.Empty);
        }
    }
}
