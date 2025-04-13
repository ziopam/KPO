using MiniDZ2.Application.Services;
using MiniDZ2.Domain.Entities;
using MiniDZ2.Domain.ValueObjects;
using MiniDZ2.Infrastructure.Interfaces;
using Moq;

namespace MiniDZ2.Tests
{
    public class RemoveAnimalFromEnclosureServiceTests
    {
        [Fact]
        public async Task RemoveAnimalAsync_ShouldRemoveAnimalFromEnclosure()
        {
            // Arrange
            var mockAnimalRepo = new Mock<IAnimalRepository>();
            var mockEnclosureRepo = new Mock<IEnclosureRepository>();

            var animal = new Animal
            {
                Species = Species.Predator,
                Name = new AnimalName("Lion"),
                BirthDate = new BirthDate("01-01-2010"),
                Gender = Gender.Male,
                FavoriteFood = new Food("Meat"),
                Status = Status.Healthy,
                IsHungry = true,
            };

            var enclosure = new Enclosure
            {
                Type = EnclosureType.forPredators,
                Size = new NoZeroPositiveInt(100),
                Capacity = new NoZeroPositiveInt(10),
                IsClean = true
            };
            animal.MoveToEnclosure(enclosure.Id);
            enclosure.AnimalIds.Add(animal.Id);

            mockAnimalRepo.Setup(repo => repo.GetByIdAsync(animal.Id)).ReturnsAsync(animal);
            mockEnclosureRepo.Setup(repo => repo.GetByIdAsync(enclosure.Id)).ReturnsAsync(enclosure);

            var service = new RemoveAnimalFromEnclosureService(mockAnimalRepo.Object, mockEnclosureRepo.Object);

            // Act
            await service.RemoveAnimalAsync(animal.Id, enclosure.Id);

            // Assert
            mockAnimalRepo.Verify(repo => repo.GetByIdAsync(animal.Id), Times.Once);
            mockEnclosureRepo.Verify(repo => repo.GetByIdAsync(enclosure.Id), Times.Once);
            Assert.Equal(Guid.Empty, animal.EnclosureId);
        }

        [Fact]
        public async Task RemoveAnimalAsync_ShouldNotThrow_WhenAnimalNotFound()
        {
            // Arrange
            var animalId = Guid.NewGuid();
            var enclosureId = Guid.NewGuid();

            var mockAnimalRepo = new Mock<IAnimalRepository>();
            var mockEnclosureRepo = new Mock<IEnclosureRepository>();

            mockAnimalRepo.Setup(repo => repo.GetByIdAsync(animalId)).ReturnsAsync((Animal?)null);
            mockEnclosureRepo.Setup(repo => repo.GetByIdAsync(enclosureId)).ReturnsAsync((Enclosure?)null);

            var service = new RemoveAnimalFromEnclosureService(mockAnimalRepo.Object, mockEnclosureRepo.Object);

            // Act
            await service.RemoveAnimalAsync(animalId, enclosureId);

            // Assert
            mockAnimalRepo.Verify(repo => repo.GetByIdAsync(animalId), Times.Once);
            mockEnclosureRepo.Verify(repo => repo.GetByIdAsync(enclosureId), Times.Once);
        }

        [Fact]
        public async Task RemoveAnimalAsync_ShouldNotThrow_WhenEnclosureNotFound()
        {
            // Arrange
            var enclosureId = Guid.NewGuid();

            var mockAnimalRepo = new Mock<IAnimalRepository>();
            var mockEnclosureRepo = new Mock<IEnclosureRepository>();

            var animal = new Animal
            {
                Species = Species.Predator,
                Name = new AnimalName("Lion"),
                BirthDate = new BirthDate("01-01-2010"),
                Gender = Gender.Male,
                FavoriteFood = new Food("Meat"),
                Status = Status.Healthy,
                IsHungry = true,
            };
            animal.MoveToEnclosure(enclosureId);

            mockAnimalRepo.Setup(repo => repo.GetByIdAsync(animal.Id)).ReturnsAsync(animal);
            mockEnclosureRepo.Setup(repo => repo.GetByIdAsync(enclosureId)).ReturnsAsync((Enclosure?)null);

            var service = new RemoveAnimalFromEnclosureService(mockAnimalRepo.Object, mockEnclosureRepo.Object);

            // Act
            await service.RemoveAnimalAsync(animal.Id, enclosureId);

            // Assert
            mockAnimalRepo.Verify(repo => repo.GetByIdAsync(animal.Id), Times.Once);
            mockEnclosureRepo.Verify(repo => repo.GetByIdAsync(enclosureId), Times.Once);
            Assert.Equal(Guid.Empty, animal.EnclosureId);
        }
    }
}
