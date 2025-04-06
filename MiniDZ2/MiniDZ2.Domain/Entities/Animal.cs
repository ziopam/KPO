using MiniDZ2.Domain.ValueObjects;

namespace MiniDZ2.Domain.Entities
{
    internal class Animal(Species species, AnimalName name, BirthDate birthDate, Gender gender, Food favoriteFood, Status status, Guid enclosureId)
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public Species Species { get; private set; } = species;
        public AnimalName Name { get; private set; } = name;
        public BirthDate BirthDate { get; private set; } = birthDate;
        public Gender Gender { get; private set; } = gender;
        public Food FavoriteFood { get; private set; } = favoriteFood;
        public Status Status { get; private set; } = status;
        public bool IsHungry { get; private set; } = true;
        public Guid EnclosureId { get; private set; } = enclosureId;

        public void Feed()
        {
            IsHungry = false;
        }
        public void Heal()
        {
            Status = Status.Healthy;
        }
        public void MoveToEnclosure(Guid newEnclosureId)
        {
            EnclosureId = newEnclosureId;
            // Raise AnimalMovedEvent
        }
    }
}
