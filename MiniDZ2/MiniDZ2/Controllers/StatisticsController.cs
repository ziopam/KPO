using Microsoft.AspNetCore.Mvc;
using MiniDZ2.Application.Interfaces;

namespace MiniDZ2.Presentation.Controllers
{
    /// <summary>
    /// Контроллер для получения статистики о зоопарке.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticsController(IZooStatisticsSerivice statisticsService) : ControllerBase
    {
        private readonly IZooStatisticsSerivice _statisticsService = statisticsService;

        /// <summary>
        /// Получить общее количество животных в зоопарке.
        /// </summary>
        [HttpGet("animals/total")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTotalAnimalCount()
        {
            var count = await _statisticsService.GetTotalAnimalCountAsync();
            return Ok(count);
        }

        /// <summary>
        /// Получить количество здоровых животных.
        /// </summary>
        [HttpGet("animals/healthy")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTotalHealthyAnimals()
        {
            var count = await _statisticsService.GetTotalHealthyAnimalsAsync();
            return Ok(count);
        }

        /// <summary>
        /// Получить количество больных животных.
        /// </summary>
        [HttpGet("animals/sick")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTotalSickAnimals()
        {
            var count = await _statisticsService.GetTotalSickAnimalsAsync();
            return Ok(count);
        }

        /// <summary>
        /// Получить общее количество вольеров.
        /// </summary>
        [HttpGet("enclosures/total")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTotalEnclosureCount()
        {
            var count = await _statisticsService.GetTotalEnclosureCountAsync();
            return Ok(count);
        }

        /// <summary>
        /// Получить количество вольеров с доступными местами.
        /// </summary>
        [HttpGet("enclosures/available")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAvailableEnclosures()
        {
            var count = await _statisticsService.GetAvailableEnclosuresAsync();
            return Ok(count);
        }

        /// <summary>
        /// Получить общее количество записей в расписании кормлений.
        /// </summary>
        [HttpGet("feedings/total")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTotalFeedingScheduleCount()
        {
            var count = await _statisticsService.GetTotalFeedingScheduleCountAsync();
            return Ok(count);
        }
    }
}
