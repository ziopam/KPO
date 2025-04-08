namespace MiniDZ2.Presentation.DTOs
{
    /// <summary>
    /// DTO для создания вольера.
    /// </summary>
    public class CreateEnclosureDto
    {
        /// <summary>
        /// Тип вольера. Может быть только из списка: [Для хищников, для травоядных, для птиц, для рыб].
        /// </summary>
        /// <example>Для хищников</example>
        public required string EnclosureType { get; set; }

        /// <summary>
        /// Размер вольера. Не может быть меньше 1.
        /// </summary>
        /// <example>1</example>
        public required int Size { get; set; }

        /// <summary>
        /// Вместимость вольера. Не может быть меньше 1.
        /// </summary>
        /// <example>1</example>
        public required int Capacity { get; set; }
    }
}
