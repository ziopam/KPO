using MediatR;
using Microsoft.AspNetCore.Mvc;
using MiniDZ2.Domain.Entities;
using MiniDZ2.Domain.Events;
using MiniDZ2.Domain.ValueObjects;
using MiniDZ2.Infrastructure.Interfaces;
using MiniDZ2.Presentation.DTOs;

namespace MiniDZ2.Presentation.Controllers
{
    /// <summary>
    /// Контроллер для управления расписанием кормления животных.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class FeedingScheduleController(IFeedingScheduleRepository feedingScheduleRepository, IAnimalRepository animalRepository, IMediator mediator) : ControllerBase
    {
        private readonly IFeedingScheduleRepository _feedingScheduleRepository = feedingScheduleRepository;
        private readonly IAnimalRepository _animalRepository = animalRepository;
        private readonly IMediator _mediator = mediator;

        /// <summary>
        /// Получить все расписания кормления.
        /// </summary>
        [ProducesResponseType(typeof(IEnumerable<FeedingSchedule>), StatusCodes.Status200OK)]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var schedules = await _feedingScheduleRepository.GetAllAsync();
            return Ok(schedules.OrderBy(s => s.FeedingTime));
        }
        /// <summary>
        /// Получить расписание кормления по ID.
        /// </summary>
        [ProducesResponseType(typeof(FeedingSchedule), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var schedule = await _feedingScheduleRepository.GetByIdAsync(id);
            if (schedule == null)
            {
                return NotFound();
            }
            return Ok(schedule);
        }

        /// <summary>
        /// Добавить новое расписание кормления.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(FeedingSchedule), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Add([FromBody] CreateFeedingScheduleDto data)
        {
            FeedingSchedule feedingSchedule;
            try
            {
                var success = DateOnly.TryParse(data.FeedingTime, out var feedingTime);
                if (!success)
                {
                    return BadRequest("Некорректный формат времени кормления. Допустимый формат dd-MM-yyyy.");
                }

                feedingSchedule = new FeedingSchedule(data.AnimalId, feedingTime, new Food(data.Food));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }

            var _ = await _animalRepository.GetByIdAsync(data.AnimalId);
            if (_ == null)
            {
                return NotFound("Животное с таким ID не найдено.");
            }

            await _feedingScheduleRepository.AddAsync(feedingSchedule);
            return CreatedAtAction(nameof(GetById), new { id = feedingSchedule.Id }, feedingSchedule);
        }

        /// <summary>
        /// Удалить расписание кормления по ID.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _feedingScheduleRepository.RemoveAsync(id);
            return NoContent();
        }

        /// <summary>
        /// Получить расписание кормления по ID животного.
        /// </summary>
        /// <param name="animalId">Уникальный идентификатор животного.</param>
        [ProducesResponseType(typeof(IEnumerable<FeedingSchedule>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [HttpGet("get-by-animal/{animalId}")]
        public async Task<IActionResult> GetByAnimalId(Guid animalId)
        {
            var schedules = await _feedingScheduleRepository.GetByAnimalIdAsync(animalId);
            if (schedules == null || !schedules.Any())
            {
                return NotFound("Расписание кормления для данного животного не найдено.");
            }
            return Ok(schedules.OrderBy(s => s.FeedingTime));
        }

        /// <summary>
        /// Покормить всех животных по конкретной дате.
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [HttpPost("feed-all-by-date")]
        public async Task<IActionResult> FeedAllByDate([FromBody] DateOnlyDto data)
        {
            var result = DateOnly.TryParse(data.Date, out DateOnly date);
            if (!result)
            {
                return BadRequest("Некорректный формат даты. Допустимый формат dd-MM-yyyy.");
            }

            var schedules = await _feedingScheduleRepository.GetByDate(date);
            if (schedules == null || !schedules.Any())
            {
                return NotFound("Расписание кормления для данной даты не найдено.");
            }

            foreach (var schedule in schedules)
            {
                await _mediator.Publish(new FeedingTimeEvent(schedule.AnimalId, schedule.Id));
            }
            return Ok("Все животные были покормлены.");
        }
    }
}
