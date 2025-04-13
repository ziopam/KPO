using MiniDZ2.Application.Interfaces;
using MiniDZ2.Application.Services;
using MiniDZ2.Domain.Entities;
using MiniDZ2.Domain.ValueObjects;
using MiniDZ2.Infrastructure.Interfaces;
using Moq;

namespace MiniDZ2.Tests
{
    public class DeleteAnimalServiceTests
    {
        [Fact]
        public void DeleteAnimal_ShouldDeleteAnimal()
        {
            // Arrange
            var enclosureId = Guid.NewGuid();

            var mockAnimalRepo = new Mock<IAnimalRepository>();
            var mockFeedingScheduleRepo = new Mock<IFeedingScheduleRepository>();
            var mockRemoveAnimalFromEnclosureService = new Mock<IRemoveAnimalFromEnclosureService>();

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


            var feedingSchedule = new FeedingSchedule(animal.Id, new DateOnly(2023, 10, 1), new Food("Meat"));

            mockAnimalRepo.Setup(repo => repo.GetByIdAsync(animal.Id)).ReturnsAsync(animal);
            mockFeedingScheduleRepo.Setup(repo => repo.GetByAnimalIdAsync(animal.Id)).ReturnsAsync(new List<FeedingSchedule> { feedingSchedule });

            var service = new DeleteAnimalService(mockAnimalRepo.Object, mockFeedingScheduleRepo.Object, mockRemoveAnimalFromEnclosureService.Object);

            // Act
            service.DeleteAnimal(animal.Id);

            // Assert
            mockAnimalRepo.Verify(repo => repo.GetByIdAsync(animal.Id), Times.Once);
            mockFeedingScheduleRepo.Verify(repo => repo.GetByAnimalIdAsync(animal.Id), Times.Once);
            mockFeedingScheduleRepo.Verify(repo => repo.RemoveAsync(feedingSchedule.Id), Times.Once);
            mockRemoveAnimalFromEnclosureService.Verify(service => service.RemoveAnimalAsync(animal.Id, enclosureId), Times.Once);
            mockAnimalRepo.Verify(repo => repo.RemoveAsync(animal.Id), Times.Once);
        }

        [Fact]
        public async Task DeleteAnimal_ShouldThrowException_WhenAnimalNotFoundAsync()
        {
            // Arrange
            var animalId = Guid.NewGuid();

            var mockAnimalRepo = new Mock<IAnimalRepository>();
            var mockFeedingScheduleRepo = new Mock<IFeedingScheduleRepository>();
            var mockRemoveAnimalFromEnclosureService = new Mock<IRemoveAnimalFromEnclosureService>();

            mockAnimalRepo.Setup(repo => repo.GetByIdAsync(animalId)).ReturnsAsync((Animal?)null);

            var service = new DeleteAnimalService(mockAnimalRepo.Object, mockFeedingScheduleRepo.Object, mockRemoveAnimalFromEnclosureService.Object);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.DeleteAnimal(animalId));
        }

        [Fact]
        public void DeleteAnimal_ShouldRemoveFeedingSchedules()
        {
            // Arrange
            var enclosureId = Guid.NewGuid();

            var mockAnimalRepo = new Mock<IAnimalRepository>();
            var mockFeedingScheduleRepo = new Mock<IFeedingScheduleRepository>();
            var mockRemoveAnimalFromEnclosureService = new Mock<IRemoveAnimalFromEnclosureService>();

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

            var feedingSchedule1 = new FeedingSchedule(animal.Id, new DateOnly(2023, 10, 1), new Food("Meat"));

            var feedingSchedule2 = new FeedingSchedule(animal.Id, new DateOnly(2023, 10, 2), new Food("Fish"));

            mockAnimalRepo.Setup(repo => repo.GetByIdAsync(animal.Id)).ReturnsAsync(animal);
            mockFeedingScheduleRepo.Setup(repo => repo.GetByAnimalIdAsync(animal.Id)).ReturnsAsync(new List<FeedingSchedule> { feedingSchedule1, feedingSchedule2 });

            var service = new DeleteAnimalService(mockAnimalRepo.Object, mockFeedingScheduleRepo.Object, mockRemoveAnimalFromEnclosureService.Object);

            // Act
            service.DeleteAnimal(animal.Id);

            // Assert
            mockFeedingScheduleRepo.Verify(repo => repo.RemoveAsync(feedingSchedule1.Id), Times.Once);
            mockFeedingScheduleRepo.Verify(repo => repo.RemoveAsync(feedingSchedule2.Id), Times.Once);
        }
    }
}

