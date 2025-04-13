using MiniDZ2.Application.Services;
using MiniDZ2.Domain.Entities;
using MiniDZ2.Domain.ValueObjects;
using MiniDZ2.Infrastructure.Interfaces;
using Moq;

namespace MiniDZ2.Tests
{
    public class FeedingOrganizationServiceTests
    {
        [Fact]
        public async Task FeedAnimal_ShouldFeedAnimalAndMarkScheduleAsCompleted()
        {
            // Arrange
            var mockAnimalRepo = new Mock<IAnimalRepository>();
            var mockFeedingScheduleRepo = new Mock<IFeedingScheduleRepository>();

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

            var feedingSchedule = new FeedingSchedule(animal.Id, new DateOnly(2023, 10, 1), new Food("Meat"));

            mockAnimalRepo.Setup(repo => repo.GetByIdAsync(animal.Id)).ReturnsAsync(animal);
            mockFeedingScheduleRepo.Setup(repo => repo.GetByIdAsync(feedingSchedule.Id)).ReturnsAsync(feedingSchedule);

            var service = new FeedingOrganizationService(mockFeedingScheduleRepo.Object, mockAnimalRepo.Object);

            // Act
            await service.FeedAnimal(animal.Id, feedingSchedule.Id);

            // Assert
            mockAnimalRepo.Verify(repo => repo.GetByIdAsync(animal.Id), Times.Once);
            mockFeedingScheduleRepo.Verify(repo => repo.GetByIdAsync(feedingSchedule.Id), Times.Once);
            Assert.False(animal.IsHungry);
            Assert.True(feedingSchedule.IsCompleted);
        }

        [Fact]
        public async Task FeedAnimal_ShouldThrowException_WhenAnimalNotFound()
        {
            // Arrange
            var animalId = Guid.NewGuid();
            var scheduleId = Guid.NewGuid();

            var mockAnimalRepo = new Mock<IAnimalRepository>();
            var mockFeedingScheduleRepo = new Mock<IFeedingScheduleRepository>();

            mockAnimalRepo.Setup(repo => repo.GetByIdAsync(animalId)).ReturnsAsync((Animal?)null);
            mockFeedingScheduleRepo.Setup(repo => repo.GetByIdAsync(scheduleId)).ReturnsAsync(new FeedingSchedule(animalId, new DateOnly(2023, 10, 1), new Food("Meat")));

            var service = new FeedingOrganizationService(mockFeedingScheduleRepo.Object, mockAnimalRepo.Object);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => service.FeedAnimal(animalId, scheduleId));
            Assert.Equal($"Животного с ID {animalId} не найдено.", exception.Message);
        }

        [Fact]
        public async Task FeedAnimal_ShouldThrowException_WhenScheduleNotFound()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();

            var mockAnimalRepo = new Mock<IAnimalRepository>();
            var mockFeedingScheduleRepo = new Mock<IFeedingScheduleRepository>();

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
            mockFeedingScheduleRepo.Setup(repo => repo.GetByIdAsync(scheduleId)).ReturnsAsync((FeedingSchedule?)null);

            var service = new FeedingOrganizationService(mockFeedingScheduleRepo.Object, mockAnimalRepo.Object);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => service.FeedAnimal(animal.Id, scheduleId));
            Assert.Equal($"Расписания с ID {scheduleId} не найдено.", exception.Message);
        }
    }
}

