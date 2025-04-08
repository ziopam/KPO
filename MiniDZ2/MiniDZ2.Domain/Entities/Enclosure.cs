using MiniDZ2.Domain.ValueObjects;

namespace MiniDZ2.Domain.Entities
{
    /// <summary>
    /// Класс, представляющий вольер в зоопарке.
    /// </summary>
    public class Enclosure
    {
        /// <summary>
        /// Уникальный идентификатор вольера.
        /// </summary>
        public Guid Id { get; private set; } = Guid.NewGuid();
        public required EnclosureType Type { get; set; }
        public required NoZeroPositiveInt Size { get; set; }
        public int AmountOfAnimals => AnimalIds.Count;
        public required NoZeroPositiveInt Capacity { get; init; }
        public bool IsClean { get; private set; } = true;
        public List<Guid> AnimalIds { get; } = [];

        public void AddAnimal(Animal animal)
        {
            if (AnimalIds.Contains(animal.Id))
            {
                return;
            }

            if (!HasAvaluablePlace())
            {
                throw new InvalidOperationException("Невозможно добавить животное. Вольер полон.");
            }

            if (!IsEnclosureSuitable(animal))
            {
                throw new InvalidOperationException("Невозможно добавить животное. Вольер не подходит по типу");
            }

            AnimalIds.Add(animal.Id);
        }

        public void RemoveAnimal(Guid animalId)
        {
            AnimalIds.Remove(animalId);
        }

        public bool HasAvaluablePlace()
        {
            return AmountOfAnimals < Capacity.Value;
        }

        public void Clean()
        {
            IsClean = true;
        }

        public void MarkAsDirty()
        {
            IsClean = false;
        }

        public bool IsEnclosureSuitable(Animal animal)
        {
            string Species = animal.Species.Value;
            string EnclosureType = Type.Value;
            var answer = Species switch
            {
                "Хищник" => EnclosureType == "Для хищников",
                "Травоядное" => EnclosureType == "Для травоядных",
                "Птица" => EnclosureType == "Для птиц",
                "Рыба" => EnclosureType == "Для рыб",
                _ => throw new ArgumentException("Неизвестный вид животного"),
            };
            return answer;
        }
    }
}
