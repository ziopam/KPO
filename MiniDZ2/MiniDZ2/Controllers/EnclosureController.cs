using Microsoft.AspNetCore.Mvc;
using MiniDZ2.Domain.Entities;
using MiniDZ2.Domain.ValueObjects;
using MiniDZ2.Infrastructure.Interfaces;
using MiniDZ2.Presentation.DTOs;

namespace MiniDZ2.Presentation.Controllers
{
    /// <summary>
    /// Контроллер для работы с вольерами.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class EnclosureController(IEnclosureRepository enclosureRepository) : ControllerBase
    {
        private readonly IEnclosureRepository _enclosureRepository = enclosureRepository;

        /// <summary>
        /// Получает всех вольеров из репозитория.
        /// </summary>
        /// <returns>Список вольеров.</returns>
        [ProducesResponseType(typeof(IEnumerable<Enclosure>), StatusCodes.Status200OK)]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var enclosures = await _enclosureRepository.GetAllAsync();
            return Ok(enclosures);
        }
        /// <summary>
        /// Получает вольер по его уникальному идентификатору из репозитория.
        /// </summary>
        /// <param name="id">Уникальный идентификатор вольера.</param>
        /// <returns>Вольер с указанным ID.</returns>
        [ProducesResponseType(typeof(Enclosure), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var enclosure = await _enclosureRepository.GetByIdAsync(id);
            if (enclosure == null)
            {
                return NotFound();
            }
            return Ok(enclosure);
        }

        /// <summary>
        /// Создает новый вольер в репозитории.
        /// </summary>
        /// <response code="201">Вольер успешно создан.</response>
        /// <response code="400">Некорректные данные.</response>
        [ProducesResponseType(typeof(Enclosure), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateEnclosureDto data)
        {
            Enclosure enclosure;
            try
            {
                enclosure = new Enclosure
                {
                    Type = EnclosureType.GetFromString(data.EnclosureType),
                    Size = new NoZeroPositiveInt(data.Size),
                    Capacity = new NoZeroPositiveInt(data.Capacity),
                };

            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }

            await _enclosureRepository.AddAsync(enclosure);
            return CreatedAtAction(nameof(GetById), new { id = enclosure.Id }, enclosure);
        }

        /// <summary>
        /// Удаляет вольер из репозитория.
        /// </summary>
        /// <param name="id">Уникальный идентификатор вольера для удаления.</param>
        [ProducesResponseType(typeof(string), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(Guid id)
        {
            var enclosure = await _enclosureRepository.GetByIdAsync(id);
            if (enclosure == null)
            {
                return NotFound();
            }
            await _enclosureRepository.RemoveAsync(id);
            return NoContent();
        }

        /// <summary>
        /// Получает вольер по уникальному идентификатору животного из репозитория.
        /// </summary>
        /// <param name="animalId">Уникальный идентификатор животного.</param>
        [ProducesResponseType(typeof(Enclosure), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [HttpGet("by-animal/{animalId}")]
        public async Task<IActionResult> GetEnclosureByAnimalId(Guid animalId)
        {
            var enclosure = await _enclosureRepository.GetEnclosureByAnimalIdAsync(animalId);
            if (enclosure == null)
            {
                return NotFound();
            }
            return Ok(enclosure);
        }
    }
}
