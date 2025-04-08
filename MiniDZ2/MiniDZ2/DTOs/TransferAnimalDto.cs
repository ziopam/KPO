namespace MiniDZ2.Presentation.DTOs
{
    /// <summary>
    /// DTO для передачи данных о перемещении животного.
    /// </summary>
    public class TransferAnimalDto
    {
        /// <summary>
        /// Уникальный идентификатор животного.
        /// </summary>
        public required Guid AnimalId { get; set; }

        /// <summary>
        /// Уникальный идентификатор вольера, в который будет перемещено животное.
        /// </summary>
        public required Guid EnclosureId { get; set; }
    }
}
