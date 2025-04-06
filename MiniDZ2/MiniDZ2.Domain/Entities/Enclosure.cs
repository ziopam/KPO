using MiniDZ2.Domain.ValueObjects;

namespace MiniDZ2.Domain.Entities
{
    internal class Enclosure(EnclosureType type, NoZeroPositiveInt size, NoZeroPositiveInt capacity)
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public EnclosureType Type { get; private set; } = type;
        public NoZeroPositiveInt Size { get; private set; } = size;
        public int AmountOfAnimals => AnimalIds.Count;
        public NoZeroPositiveInt Capacity { get; private set; } = capacity;
        public List<Guid> AnimalIds { get; private set; } = [];

        public void AddAnimal(Guid animalId)
        {
            if (AnimalIds.Contains(animalId))
            {
                return;
            }

            if (AmountOfAnimals == Capacity.Value)
            {
                throw new InvalidOperationException("Невозможно добавить животное. Клетка полная");
            }

            AnimalIds.Add(animalId);
        }

        public void RemoveAnimal(Guid animalId)
        {
            AnimalIds.Remove(animalId);
        }

        public void Clean()
        {
        }
    }
}
