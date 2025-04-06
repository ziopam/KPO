using Microsoft.AspNetCore.Mvc;
using MiniDZ2.Application.DTOs;
using MiniDZ2.Domain.Entities;
using MiniDZ2.Infrastructure.Interfaces;

namespace MiniDZ2.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnimalController(IAnimalRepository animalRepository) : ControllerBase
    {
        private readonly IAnimalRepository _animalRepository = animalRepository;

        // GET api/animal
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var animals = await _animalRepository.GetAllAsync();
            return Ok(animals);
        }

        // GET api/animal/{id}
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

        // POST api/animal
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAnimalDto animal)
        {
            try
            {
                Animal animal1 = new Animal
                {
                    Id = Guid.NewGuid(),
                    Name = animal.Name,
                    Species = animal.Species,
                    Age = animal.Age,
                    EnclosureId = animal.EnclosureId
                };
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }

            await _animalRepository.AddAsync(animal);
            return CreatedAtAction(nameof(GetById), new { id = animal.Id }, animal);
        }

        // DELETE api/animal/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var animal = await _animalRepository.GetByIdAsync(id);
            if (animal == null)
            {
                return NotFound();
            }
            await _animalRepository.RemoveAsync(animal.Id);
            return NoContent();
        }
    }
}
