using Microsoft.AspNetCore.Mvc;
using MiniDZ2.Application.DTOs;
using MiniDZ2.Application.Interfaces;
using MiniDZ2.Domain.Entities;
using MiniDZ2.Domain.ValueObjects;
using MiniDZ2.Infrastructure.Interfaces;
using MiniDZ2.Presentation.DTOs;

namespace MiniDZ2.Presentation.Controllers
{
    /// <summary>
    /// Контроллер для работы с животными.
    /// </summary>
    /// <param name="animalRepository">Репозиторий для работы с БД животных.</param>
    /// <param name="animalTransferService"></param>
    /// <param name="deleteAnimalService"></param>
    [Route("api/[controller]")]
    [ApiController]
    public class AnimalController(IAnimalRepository animalRepository, IAnimalTransferService animalTransferService, IDeleteAnimalService deleteAnimalService) : ControllerBase
    {
        private readonly IAnimalRepository _animalRepository = animalRepository;

        /// <summary>
        /// Получает всех животных из репозитория.
        /// </summary>
        /// <returns>Список животных.</returns>
        [ProducesResponseType(typeof(IEnumerable<Animal>), StatusCodes.Status200OK)]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var animals = await _animalRepository.GetAllAsync();
            return Ok(animals);
        }

        /// <summary>
        /// Получает животное по его уникальному идентификатору из репозитория.
        /// </summary>
        /// <param name="id">Уникальный идентификатор животного.</param>
        /// <returns>Животное с указанным ID.</returns>
        [ProducesResponseType(typeof(Animal), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var animal = await _animalRepository.GetByIdAsync(id);
            if (animal == null)
            {
                return NotFound();
            }
            return Ok(animal);
        }

        /// <summary>
        /// Создает новое животное в репозитории.
        /// </summary>
        /// <response code="201">Животное успешно создано.</response>
        /// <response code="400">Некорректные данные.</response>
        [ProducesResponseType(typeof(Animal), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAnimalDto data)
        {
            Animal animal;
            try
            {
                animal = new()
                {
                    Species = Species.GetSpeciesByString(data.Species),
                    Name = new AnimalName(data.Name),
                    BirthDate = new BirthDate(data.BirthDate),
                    Gender = Gender.GetGenderByString(data.Gender),
                    FavoriteFood = new Food(data.FavoriteFood),
                    Status = Status.GetStatusByString(data.Status),
                    IsHungry = data.IsHungry,
                };
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }

            await _animalRepository.AddAsync(animal);
            return CreatedAtAction(nameof(GetById), new { id = animal.Id }, animal);
        }

        /// <summary>
        /// Удаляет животное из репозитория.
        /// </summary>
        /// <param name="id">Уникальный идентификатор животного.</param>
        [ProducesResponseType(typeof(string), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var animal = await _animalRepository.GetByIdAsync(id);
            if (animal == null)
            {
                return NotFound();
            }

            deleteAnimalService.DeleteAnimal(id);
            return NoContent();
        }

        /// <summary>
        /// Перемещает животное в другой вольер.
        /// </summary>
        /// <response code="204">Животное успешно перемещено.</response>
        /// <response code="400">Ошибка при перемещении животного. Вольер заполнен, вольер не подходит животному по типу или животное уже в вольере</response>
        /// <response code="404">Животное или вальер не найдены.</response>
        [HttpPatch("transfer")]
        [ProducesResponseType(typeof(string), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> TransferAnimalToEnclosure([FromBody] TransferAnimalDto data)
        {
            try
            {
                await animalTransferService.MoveAnimalAsync(data.AnimalId, data.EnclosureId);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            return NoContent();
        }

        /// <summary>
        /// Отмечает животное как голодное.
        /// </summary>
        /// <param name="id">Уникальный идентификатор животного</param>
        [ProducesResponseType(typeof(string), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [HttpPatch("mark-as-hungry/{id}")]
        public async Task<IActionResult> MarkAsHungry(Guid id)
        {
            var animal = await _animalRepository.GetByIdAsync(id);
            if (animal == null)
            {
                return NotFound();
            }
            animal.MarkAsHungry();
            await _animalRepository.AddAsync(animal);
            return NoContent();
        }

        /// <summary>
        /// Отмечает животное как сытое.
        /// </summary>
        /// <param name="id">Уникальный идентификатор животного.</param>
        [ProducesResponseType(typeof(string), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [HttpPatch("feed/{id}")]
        public async Task<IActionResult> FeedAnimal(Guid id)
        {
            var animal = await _animalRepository.GetByIdAsync(id);
            if (animal == null)
            {
                return NotFound();
            }
            animal.Feed();
            await _animalRepository.AddAsync(animal);
            return NoContent();
        }

        /// <summary>
        /// Отмечает животное как больное.
        /// </summary>
        /// <param name="id">Уникальный идентификатор животного.</param>
        [ProducesResponseType(typeof(string), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [HttpPatch("mark-as-sick/{id}")]
        public async Task<IActionResult> MarkAsSick(Guid id)
        {
            var animal = await _animalRepository.GetByIdAsync(id);
            if (animal == null)
            {
                return NotFound();
            }
            animal.MarkAsSick();
            await _animalRepository.AddAsync(animal);
            return NoContent();
        }


        /// <summary>
        /// Отмечает животное как здоровое.
        /// </summary>
        /// <param name="id">Уникальный идентификатор животного.</param>
        [ProducesResponseType(typeof(string), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [HttpPatch("heal-animal/{id}")]
        public async Task<IActionResult> HealAnimal(Guid id)
        {
            var animal = await _animalRepository.GetByIdAsync(id);
            if (animal == null)
            {
                return NotFound();
            }
            animal.Heal();
            await _animalRepository.AddAsync(animal);
            return NoContent();
        }
    }
}
