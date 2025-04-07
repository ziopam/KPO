using System.ComponentModel.DataAnnotations;

namespace MiniDZ2.Application.DTOs
{
    /// <summary>
    /// DTO для создания животного.
    /// </summary>
    public class CreateAnimalDto
    {
        /// <summary>
        /// Название вида.
        /// </summary>
        /// <example>Лев</example>
        [Required]
        public required string Species { get; set; }

        /// <summary>
        /// Имя животного.
        /// </summary>
        /// <example>Лёва</example>
        [Required]
        public required string Name { get; set; }

        /// <summary>
        /// Дата рождения. Не может быть позже сегодняшнего дня. Формат dd-MM-yyyy.
        /// </summary>
        /// <example>28-10-2005</example>
        [Required]
        public required string BirthDate { get; set; }

        /// <summary>
        /// Пол животного. Может быть только из списка: [Самка, Самец].
        /// </summary>
        /// <example>Самец</example>
        [Required]
        public required string Gender { get; set; }

        /// <summary>
        /// Любимая еда.
        /// </summary>
        /// <example>Мясо</example>
        [Required]
        public required string FavoriteFood { get; set; }

        /// <summary>
        /// Статус животного. Может быть только из списка: [Здоров, Болен].
        /// </summary>
        /// <example>Здоров</example>
        [Required]
        public required string Status { get; set; }

        /// <summary>
        /// Голодное ли животное.
        /// </summary>
        /// <example>true</example>
        [Required]
        public required bool IsHungry { get; set; }

        /// <summary>
        /// Опасное ли животное.
        /// </summary>
        /// <example>true</example>
        [Required]
        public required bool IsDangerous { get; set; }
    }
}
