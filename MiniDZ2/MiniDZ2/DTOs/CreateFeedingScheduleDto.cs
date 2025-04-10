namespace MiniDZ2.Presentation.DTOs
{
    /// <summary>
    /// DTO для создания расписания кормления.
    /// </summary>
    public class CreateFeedingScheduleDto
    {
        /// <summary>
        /// ID животного.
        /// </summary>
        public required Guid AnimalId { get; set; }

        /// <summary>
        /// Время кормления. Формат dd-MM-yyyy.
        /// </summary>
        /// <example>28-10-2023</example>
        public required string FeedingTime { get; set; }

        /// <summary>
        /// Пища для животного.
        /// </summary>
        /// <example>Мясо</example>
        public required string Food { get; set; }
    }
}
