namespace MiniDZ2.Presentation.DTOs
{
    /// <summary>
    /// DTO для передачи даты в формате "dd-MM-yyyy".
    /// </summary>
    public class DateOnlyDto
    {
        /// <summary>
        /// Дата в формате "dd-MM-yyyy".
        /// </summary>
        /// <example>28-10-2023</example>
        public required string Date { get; set; }
    }
}
