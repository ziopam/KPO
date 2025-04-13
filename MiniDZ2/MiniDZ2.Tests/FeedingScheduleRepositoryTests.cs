using MiniDZ2.Domain.Entities;
using MiniDZ2.Domain.ValueObjects;
using MiniDZ2.Infrastructure.Repositories;

namespace MiniDZ2.Tests
{
    public class FeedingScheduleRepositoryTests
    {
        [Fact]
        public async Task AddAsync_ShouldAddFeedingSchedule()
        {
            // Arrange
            var repository = new FeedingScheduleRepository();
            var feedingSchedule = new FeedingSchedule(Guid.NewGuid(), new DateOnly(2023, 10, 1), new Food("Meat"));

            // Act
            var result = await repository.AddAsync(feedingSchedule);

            // Assert
            Assert.True(result);
            var addedSchedule = await repository.GetByIdAsync(feedingSchedule.Id);
            Assert.NotNull(addedSchedule);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllFeedingSchedules()
        {
            // Arrange
            var repository = new FeedingScheduleRepository();
            var feedingSchedule1 = new FeedingSchedule(Guid.NewGuid(), new DateOnly(2023, 10, 1), new Food("Meat"));
            var feedingSchedule2 = new FeedingSchedule(Guid.NewGuid(), new DateOnly(2023, 10, 2), new Food("Fish"));

            await repository.AddAsync(feedingSchedule1);
            await repository.AddAsync(feedingSchedule2);

            // Act
            var schedules = await repository.GetAllAsync();

            // Assert
            Assert.Contains(feedingSchedule1, schedules);
            Assert.Contains(feedingSchedule2, schedules);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnFeedingSchedule_WhenScheduleExists()
        {
            // Arrange
            var repository = new FeedingScheduleRepository();
            var feedingSchedule = new FeedingSchedule(Guid.NewGuid(), new DateOnly(2023, 10, 1), new Food("Meat"));

            await repository.AddAsync(feedingSchedule);

            // Act
            var retrievedSchedule = await repository.GetByIdAsync(feedingSchedule.Id);

            // Assert
            Assert.NotNull(retrievedSchedule);
            Assert.Equal(feedingSchedule.Id, retrievedSchedule.Id);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenScheduleDoesNotExist()
        {
            // Arrange
            var repository = new FeedingScheduleRepository();
            var nonExistentId = Guid.NewGuid();

            // Act
            var schedule = await repository.GetByIdAsync(nonExistentId);

            // Assert
            Assert.Null(schedule);
        }

        [Fact]
        public async Task GetByAnimalIdAsync_ShouldReturnFeedingSchedules_WhenAnimalHasSchedules()
        {
            // Arrange
            var repository = new FeedingScheduleRepository();
            var animalId = Guid.NewGuid();
            var feedingSchedule1 = new FeedingSchedule(animalId, new DateOnly(2023, 10, 1), new Food("Meat"));
            var feedingSchedule2 = new FeedingSchedule(animalId, new DateOnly(2023, 10, 2), new Food("Fish"));

            await repository.AddAsync(feedingSchedule1);
            await repository.AddAsync(feedingSchedule2);

            // Act
            var schedules = await repository.GetByAnimalIdAsync(animalId);

            // Assert
            Assert.Contains(feedingSchedule1, schedules);
            Assert.Contains(feedingSchedule2, schedules);
        }

        [Fact]
        public async Task GetByAnimalIdAsync_ShouldReturnEmpty_WhenAnimalHasNoSchedules()
        {
            // Arrange
            var repository = new FeedingScheduleRepository();
            var animalId = Guid.NewGuid();

            // Act
            var schedules = await repository.GetByAnimalIdAsync(animalId);

            // Assert
            Assert.Empty(schedules);
        }

        [Fact]
        public async Task GetByDate_ShouldReturnFeedingSchedules_WhenSchedulesExistForDate()
        {
            // Arrange
            var repository = new FeedingScheduleRepository();
            var date = new DateOnly(2023, 10, 1);
            var feedingSchedule1 = new FeedingSchedule(Guid.NewGuid(), date, new Food("Meat"));
            var feedingSchedule2 = new FeedingSchedule(Guid.NewGuid(), date, new Food("Fish"));

            await repository.AddAsync(feedingSchedule1);
            await repository.AddAsync(feedingSchedule2);

            // Act
            var schedules = await repository.GetByDate(date);

            // Assert
            Assert.Contains(feedingSchedule1, schedules);
            Assert.Contains(feedingSchedule2, schedules);
        }

        [Fact]
        public async Task GetByDate_ShouldReturnEmpty_WhenNoSchedulesExistForDate()
        {
            // Arrange
            var repository = new FeedingScheduleRepository();
            var date = new DateOnly(2023, 10, 1);

            // Act
            var schedules = await repository.GetByDate(date);

            // Assert
            Assert.Empty(schedules);
        }

        [Fact]
        public async Task RemoveAsync_ShouldRemoveFeedingSchedule()
        {
            // Arrange
            var repository = new FeedingScheduleRepository();
            var feedingSchedule = new FeedingSchedule(Guid.NewGuid(), new DateOnly(2023, 10, 1), new Food("Meat"));

            await repository.AddAsync(feedingSchedule);

            // Act
            await repository.RemoveAsync(feedingSchedule.Id);
            var removedSchedule = await repository.GetByIdAsync(feedingSchedule.Id);

            // Assert
            Assert.Null(removedSchedule);
        }
    }
}
