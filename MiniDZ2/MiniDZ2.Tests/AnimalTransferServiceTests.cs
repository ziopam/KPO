using MediatR;
using MiniDZ2.Application.Services;
using MiniDZ2.Domain.Entities;
using MiniDZ2.Domain.Events;
using MiniDZ2.Domain.ValueObjects;
using MiniDZ2.Infrastructure.Interfaces;
using Moq;

namespace MiniDZ2.Tests
{
    public class AnimalTransferServiceTests
    {
        [Fact]
        public async Task MoveAnimalAsync_ShouldMoveAnimal()
        {
            // Arrange
            var mockAnimalRepo = new Mock<IAnimalRepository>();
            var mockEnclosureRepo = new Mock<IEnclosureRepository>();
            var mockMediator = new Mock<IMediator>();

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

            var toEnclosure = new Enclosure
            {
                Type = EnclosureType.forPredators,
                Size = new NoZeroPositiveInt(100),
                Capacity = new NoZeroPositiveInt(10),
                IsClean = true,
            };
            mockAnimalRepo.Setup(repo => repo.GetByIdAsync(animal.Id)).ReturnsAsync(animal);
            mockEnclosureRepo.Setup(repo => repo.GetByIdAsync(toEnclosure.Id)).ReturnsAsync(toEnclosure);

            var service = new AnimalTransferService(mockAnimalRepo.Object, mockEnclosureRepo.Object, mockMediator.Object);

            // Act
            await service.MoveAnimalAsync(animal.Id, toEnclosure.Id);

            // Assert
            mockEnclosureRepo.Verify(repo => repo.GetByIdAsync(toEnclosure.Id), Times.Once);
            mockAnimalRepo.Verify(repo => repo.GetByIdAsync(animal.Id), Times.Once);
            mockMediator.Verify(m => m.Publish(It.IsAny<AnimalMovedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
            Assert.Contains(animal.Id, toEnclosure.AnimalIds);
            Assert.Equal(toEnclosure.Id, animal.EnclosureId);
        }

        [Fact]
        public async Task MoveAnimalAsync_ShouldThrowException_WhenAnimalNotFound()
        {
            // Arrange
            var animalId = Guid.NewGuid();
            var toEnclosureId = Guid.NewGuid();

            var mockAnimalRepo = new Mock<IAnimalRepository>();
            var mockEnclosureRepo = new Mock<IEnclosureRepository>();
            var mockMediator = new Mock<IMediator>();

            mockAnimalRepo.Setup(repo => repo.GetByIdAsync(animalId)).ReturnsAsync((Animal?)null);

            var service = new AnimalTransferService(mockAnimalRepo.Object, mockEnclosureRepo.Object, mockMediator.Object);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.MoveAnimalAsync(animalId, toEnclosureId));
        }

        [Fact]
        public async Task MoveAnimalAsync_ShouldThrowException_WhenToEnclosureNotFound()
        {
            // Arrange
            var toEnclosureId = Guid.NewGuid();

            var mockAnimalRepo = new Mock<IAnimalRepository>();
            var mockEnclosureRepo = new Mock<IEnclosureRepository>();
            var mockMediator = new Mock<IMediator>();

            var animal = new Animal
            {
                Species = Species.Predator,
                Name = new AnimalName("Lion"),
                BirthDate = new BirthDate("01-01-2010"),
                Gender = Gender.Male,
                FavoriteFood = new Food("Meat"),
                Status = Status.Healthy,
                IsHungry = true
            };

            mockAnimalRepo.Setup(repo => repo.GetByIdAsync(animal.Id)).ReturnsAsync(animal);
            mockEnclosureRepo.Setup(repo => repo.GetByIdAsync(toEnclosureId)).ReturnsAsync((Enclosure?)null);

            var service = new AnimalTransferService(mockAnimalRepo.Object, mockEnclosureRepo.Object, mockMediator.Object);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.MoveAnimalAsync(animal.Id, toEnclosureId));
        }

        [Fact]
        public async Task MoveAnimalAsync_ShouldThrowException_WhenAnimalAlreadyInToEnclosure()
        {
            // Arrange
            var mockAnimalRepo = new Mock<IAnimalRepository>();
            var mockEnclosureRepo = new Mock<IEnclosureRepository>();
            var mockMediator = new Mock<IMediator>();

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

            var toEnclosure = new Enclosure
            {
                Type = EnclosureType.forPredators,
                Size = new NoZeroPositiveInt(100),
                Capacity = new NoZeroPositiveInt(10),
                IsClean = true,
            };
            animal.MoveToEnclosure(toEnclosure.Id);

            mockAnimalRepo.Setup(repo => repo.GetByIdAsync(animal.Id)).ReturnsAsync(animal);
            mockEnclosureRepo.Setup(repo => repo.GetByIdAsync(toEnclosure.Id)).ReturnsAsync(toEnclosure);
            mockEnclosureRepo.Setup(repo => repo.GetEnclosureByAnimalIdAsync(animal.Id)).ReturnsAsync(toEnclosure);

            var service = new AnimalTransferService(mockAnimalRepo.Object, mockEnclosureRepo.Object, mockMediator.Object);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => service.MoveAnimalAsync(animal.Id, toEnclosure.Id));
        }

        [Fact]
        public async Task MoveAnimalAsync_ShouldThrowException_WhenToEnclosureHasNoSpace()
        {
            // Arrange
            var mockAnimalRepo = new Mock<IAnimalRepository>();
            var mockEnclosureRepo = new Mock<IEnclosureRepository>();
            var mockMediator = new Mock<IMediator>();

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

            var fromEnclosure = new Enclosure
            {
                Type = EnclosureType.forPredators,
                Size = new NoZeroPositiveInt(100),
                Capacity = new NoZeroPositiveInt(10),
                IsClean = true,
            };
            animal.MoveToEnclosure(fromEnclosure.Id);

            var toEnclosure = new Enclosure
            {
                Type = EnclosureType.forPredators,
                Size = new NoZeroPositiveInt(100),
                Capacity = new NoZeroPositiveInt(1),
                IsClean = true
            };
            toEnclosure.AnimalIds.Add(Guid.NewGuid());

            mockAnimalRepo.Setup(repo => repo.GetByIdAsync(animal.Id)).ReturnsAsync(animal);
            mockEnclosureRepo.Setup(repo => repo.GetEnclosureByAnimalIdAsync(animal.Id)).ReturnsAsync(fromEnclosure);
            mockEnclosureRepo.Setup(repo => repo.GetByIdAsync(toEnclosure.Id)).ReturnsAsync(toEnclosure);

            var service = new AnimalTransferService(mockAnimalRepo.Object, mockEnclosureRepo.Object, mockMediator.Object);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.MoveAnimalAsync(animal.Id, toEnclosure.Id));
        }
    }
}