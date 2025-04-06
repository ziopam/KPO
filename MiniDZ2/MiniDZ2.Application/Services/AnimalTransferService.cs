using MediatR;
using MiniDZ2.Domain.Events;
using MiniDZ2.Infrastructure.Interfaces;

namespace MiniDZ2.Application.Services
{
    public class AnimalTransferService(IAnimalRepository animalRepo, IEnclosureRepository enclosureRepo, IMediator mediator)
    {
        private readonly IAnimalRepository _animalRepo = animalRepo;
        private readonly IEnclosureRepository _enclosureRepo = enclosureRepo;
        private readonly IMediator _mediator = mediator;

        public async Task MoveAnimalAsync(Guid animalId, Guid toEnclosureId)
        {
            var animal = await _animalRepo.GetByIdAsync(animalId)
                ?? throw new ArgumentException("Животное не найдено");

            var fromEnclosure = await _enclosureRepo.GetEnclosureByAnimalIdAsync(animalId) ??
                throw new ArgumentException("Вольер отправления не найден");
            var toEnclosure = await _enclosureRepo.GetByIdAsync(toEnclosureId)
                ?? throw new ArgumentException("Вольер назначения не найден");

            if (!toEnclosure.HasAvaluablePlace())
            {
                throw new ArgumentException("Нет места в вольере назначения");
            }

            fromEnclosure.RemoveAnimal(animal.Id);
            toEnclosure.AddAnimal(animal.Id);
            animal.MoveToEnclosure(toEnclosure.Id);

            var @event = new AnimalMovedEvent(animal.Id, fromEnclosure.Id, toEnclosure.Id);
            await _mediator.Publish(@event);
        }
    }
}
