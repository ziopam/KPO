using MiniDZ2.Application.Interfaces;
using MiniDZ2.Infrastructure.Interfaces;

namespace MiniDZ2.Application.Services
{
    public class DeleteAnimalService(IAnimalRepository animalRepository, IFeedingScheduleRepository feedingScheduleRepository, IRemoveAnimalFromEnclosureService removeAnimalFromEnclosureService) : IDeleteAnimalService
    {
        private readonly IAnimalRepository _animalRepository = animalRepository;
        private readonly IFeedingScheduleRepository _feedingScheduleRepository = feedingScheduleRepository;
        private readonly IRemoveAnimalFromEnclosureService _removeAnimalFromEnclosureService = removeAnimalFromEnclosureService;

        public async Task DeleteAnimal(Guid id)
        {
            var animal = await _animalRepository.GetByIdAsync(id);
            if (animal == null)
            {
                throw new ArgumentException($"Животного с ID {id} не найдено.");
            }

            var feedingSchedules = await _feedingScheduleRepository.GetByAnimalIdAsync(id);
            if (feedingSchedules != null)
            {
                foreach (var schedule in feedingSchedules)
                {
                    await _feedingScheduleRepository.RemoveAsync(schedule.Id);
                }
            }

            await _removeAnimalFromEnclosureService.RemoveAnimalAsync(animal.Id, animal.EnclosureId);
            await _animalRepository.RemoveAsync(id);
        }
    }
}
