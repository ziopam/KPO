using System.ComponentModel.DataAnnotations;

namespace MiniDZ2.Domain.ValueObjects
{
    /// <summary>
    /// ValueObject для имени животного.
    /// </summary>
    public record AnimalName
    {
        /// <summary>
        /// Имя животного.
        /// </summary>
        /// <example>Лёва</example>
        [Required]
        public string Value { get; }

        /// <summary>
        /// Создает новое имя животного.
        /// </summary>
        /// <param name="name">Имя животного.</param>
        /// <exception cref="ArgumentException">Происходит, когда передано пустое имя животного.</exception>
        public AnimalName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Имя животного не может быть пустым");
            }
            Value = name;
        }
    }
}
