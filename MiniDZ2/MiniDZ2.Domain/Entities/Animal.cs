using MiniDZ2.Domain.ValueObjects;

namespace MiniDZ2.Domain.Entities
{
    /// <summary>
    /// Класс, представляющий животное в зоопарке.
    /// </summary>
    public class Animal
    {
        /// <summary>
        /// Уникальный идентификатор животного.
        /// </summary>
        public Guid Id { get; private set; } = Guid.NewGuid();
        public required Species Species { get; set; }
        public required AnimalName Name { get; set; }
        public required BirthDate BirthDate { get; set; }
        public required Gender Gender { get; set; }
        public required Food FavoriteFood { get; set; }
        public required Status Status { get; set; }
        public required bool IsHungry { get; set; } = true;
        public Guid EnclosureId { get; private set; } = Guid.Empty;

        /// <summary>
        /// Покормить животное.
        /// </summary>
        public void Feed()
        {
            IsHungry = false;
        }

        /// <summary>
        /// Исцелить животное.
        /// </summary>
        public void Heal()
        {
            Status = Status.Healthy;
        }

        /// <summary>
        /// Переместить животное в другую клетку.
        /// </summary>
        /// <param name="newEnclosureId">Id новой клетки.</param>
        public void MoveToEnclosure(Guid newEnclosureId)
        {
            EnclosureId = newEnclosureId;
        }

        /// <summary>
        /// Проверяет, находится ли животное в клетке или нет.
        /// </summary>
        /// <returns>true - если животное в клетке, false - иначе</returns>
        public bool IsInEnclosure()
        {
            return EnclosureId != Guid.Empty;
        }
    }
}
