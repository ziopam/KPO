using MiniDZ2.Application.Services;
using MiniDZ2.Domain.Entities;
using MiniDZ2.Domain.ValueObjects;
using MiniDZ2.Infrastructure.Interfaces;
using Moq;

namespace MiniDZ2.Tests
{
    public class ZooStatisticsServiceTests
    {
        [Fact]
        public async Task GetTotalAnimalCountAsync_ShouldReturnTotalAnimalCount()
        {
            // Arrange
            var mockAnimalRepo = new Mock<IAnimalRepository>();
            var mockEnclosureRepo = new Mock<IEnclosureRepository>();
            var mockFeedingScheduleRepo = new Mock<IFeedingScheduleRepository>();

            var animals = new List<Animal>
            {
                new() {
                    Species = Species.Predator,
                    Name = new AnimalName("Lion"),
                    BirthDate = new BirthDate("01-01-2010"),
                    Gender = Gender.Male,
                    FavoriteFood = new Food("Meat"),
                    Status = Status.Healthy,
                    IsHungry = true
                },
                new() {
                    Species = Species.Predator,
                    Name = new AnimalName("Tiger"),
                    BirthDate = new BirthDate("01-01-2011"),
                    Gender = Gender.Female,
                    FavoriteFood = new Food("Meat"),
                    Status = Status.Healthy,
                    IsHungry = true
                }
            };

            mockAnimalRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(animals);

            var service = new ZooStatisticsService(mockAnimalRepo.Object, mockEnclosureRepo.Object, mockFeedingScheduleRepo.Object);

            // Act
            var result = await service.GetTotalAnimalCountAsync();

            // Assert
            Assert.Equal(2, result);
        }

        [Fact]
        public async Task GetTotalHealthyAnimalsAsync_ShouldReturnTotalHealthyAnimals()
        {
            // Arrange
            var mockAnimalRepo = new Mock<IAnimalRepository>();
            var mockEnclosureRepo = new Mock<IEnclosureRepository>();
            var mockFeedingScheduleRepo = new Mock<IFeedingScheduleRepository>();

            var animals = new List<Animal>
            {
                new() {
                    Species = Species.Predator,
                    Name = new AnimalName("Lion"),
                    BirthDate = new BirthDate("01-01-2010"),
                    Gender = Gender.Male,
                    FavoriteFood = new Food("Meat"),
                    Status = Status.Healthy,
                    IsHungry = true
                },
                new() {
                    Species = Species.Predator,
                    Name = new AnimalName("Tiger"),
                    BirthDate = new BirthDate("01-01-2011"),
                    Gender = Gender.Female,
                    FavoriteFood = new Food("Meat"),
                    Status = Status.Sick,
                    IsHungry = true
                }
            };

            mockAnimalRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(animals);

            var service = new ZooStatisticsService(mockAnimalRepo.Object, mockEnclosureRepo.Object, mockFeedingScheduleRepo.Object);

            // Act
            var result = await service.GetTotalHealthyAnimalsAsync();

            // Assert
            Assert.Equal(1, result);
        }

        [Fact]
        public async Task GetTotalSickAnimalsAsync_ShouldReturnTotalSickAnimals()
        {
            // Arrange
            var mockAnimalRepo = new Mock<IAnimalRepository>();
            var mockEnclosureRepo = new Mock<IEnclosureRepository>();
            var mockFeedingScheduleRepo = new Mock<IFeedingScheduleRepository>();

            var animals = new List<Animal>
            {
                new() {
                    Species = Species.Predator,
                    Name = new AnimalName("Lion"),
                    BirthDate = new BirthDate("01-01-2010"),
                    Gender = Gender.Male,
                    FavoriteFood = new Food("Meat"),
                    Status = Status.Healthy,
                    IsHungry = true
                },
                new() {
                    Species = Species.Predator,
                    Name = new AnimalName("Tiger"),
                    BirthDate = new BirthDate("01-01-2011"),
                    Gender = Gender.Female,
                    FavoriteFood = new Food("Meat"),
                    Status = Status.Sick,
                    IsHungry = true
                }
            };

            mockAnimalRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(animals);

            var service = new ZooStatisticsService(mockAnimalRepo.Object, mockEnclosureRepo.Object, mockFeedingScheduleRepo.Object);

            // Act
            var result = await service.GetTotalSickAnimalsAsync();

            // Assert
            Assert.Equal(1, result);
        }

        [Fact]
        public async Task GetTotalEnclosureCountAsync_ShouldReturnTotalEnclosureCount()
        {
            // Arrange
            var mockAnimalRepo = new Mock<IAnimalRepository>();
            var mockEnclosureRepo = new Mock<IEnclosureRepository>();
            var mockFeedingScheduleRepo = new Mock<IFeedingScheduleRepository>();

            var enclosures = new List<Enclosure>
            {
                new() {
                Type = EnclosureType.forPredators,
                Size = new NoZeroPositiveInt(100),
                Capacity = new NoZeroPositiveInt(10),
                IsClean = true
                },
                new() {
                Type = EnclosureType.forPredators,
                Size = new NoZeroPositiveInt(100),
                Capacity = new NoZeroPositiveInt(10),
                IsClean = true
                }
            };

            mockEnclosureRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(enclosures);

            var service = new ZooStatisticsService(mockAnimalRepo.Object, mockEnclosureRepo.Object, mockFeedingScheduleRepo.Object);

            // Act
            var result = await service.GetTotalEnclosureCountAsync();

            // Assert
            Assert.Equal(2, result);
        }

        [Fact]
        public async Task GetAvailableEnclosuresAsync_ShouldReturnAvailableEnclosures()
        {
            // Arrange
            var mockAnimalRepo = new Mock<IAnimalRepository>();
            var mockEnclosureRepo = new Mock<IEnclosureRepository>();
            var mockFeedingScheduleRepo = new Mock<IFeedingScheduleRepository>();

            var enclosures = new List<Enclosure>
            {
                new() {
                Type = EnclosureType.forPredators,
                Size = new NoZeroPositiveInt(100),
                Capacity = new NoZeroPositiveInt(10),
                IsClean = true
                },
                new() {
                Type = EnclosureType.forPredators,
                Size = new NoZeroPositiveInt(100),
                Capacity = new NoZeroPositiveInt(10),
                IsClean = true
                }
            };

            mockEnclosureRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(enclosures);

            var service = new ZooStatisticsService(mockAnimalRepo.Object, mockEnclosureRepo.Object, mockFeedingScheduleRepo.Object);

            // Act
            var result = await service.GetAvailableEnclosuresAsync();

            // Assert
            Assert.Equal(2, result);
        }

        [Fact]
        public async Task GetTotalFeedingScheduleCountAsync_ShouldReturnTotalFeedingScheduleCount()
        {
            // Arrange
            var mockAnimalRepo = new Mock<IAnimalRepository>();
            var mockEnclosureRepo = new Mock<IEnclosureRepository>();
            var mockFeedingScheduleRepo = new Mock<IFeedingScheduleRepository>();

            var feedingSchedules = new List<FeedingSchedule>
            {
                new(Guid.NewGuid(), new DateOnly(2023, 10, 1), new Food("Meat")),
                new(Guid.NewGuid(), new DateOnly(2023, 10, 2), new Food("Fish"))
            };

            mockFeedingScheduleRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(feedingSchedules);

            var service = new ZooStatisticsService(mockAnimalRepo.Object, mockEnclosureRepo.Object, mockFeedingScheduleRepo.Object);

            // Act
            var result = await service.GetTotalFeedingScheduleCountAsync();

            // Assert
            Assert.Equal(2, result);
        }
    }
}
