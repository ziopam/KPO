using MiniDZ2.Domain.Entities;
using MiniDZ2.Domain.ValueObjects;
using MiniDZ2.Infrastructure.Repositories;

namespace MiniDZ2.Tests
{
    public class EnclosureRepositoryTests
    {
        [Fact]
        public async Task AddAsync_ShouldAddEnclosure()
        {
            // Arrange
            var repository = new EnclosureRepository();
            var enclosure = new Enclosure
            {
                Type = EnclosureType.forPredators,
                Size = new NoZeroPositiveInt(100),
                Capacity = new NoZeroPositiveInt(10),
                IsClean = true
            };

            // Act
            var result = await repository.AddAsync(enclosure);

            // Assert
            Assert.True(result);
            var addedEnclosure = await repository.GetByIdAsync(enclosure.Id);
            Assert.NotNull(addedEnclosure);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllEnclosures()
        {
            // Arrange
            var repository = new EnclosureRepository();
            var enclosure1 = new Enclosure
            {
                Type = EnclosureType.forPredators,
                Size = new NoZeroPositiveInt(100),
                Capacity = new NoZeroPositiveInt(10),
                IsClean = true
            };
            var enclosure2 = new Enclosure
            {
                Type = EnclosureType.forHerbivores,
                Size = new NoZeroPositiveInt(200),
                Capacity = new NoZeroPositiveInt(20),
                IsClean = true
            };

            await repository.AddAsync(enclosure1);
            await repository.AddAsync(enclosure2);

            // Act
            var enclosures = await repository.GetAllAsync();

            // Assert
            Assert.Contains(enclosure1, enclosures);
            Assert.Contains(enclosure2, enclosures);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnEnclosure_WhenEnclosureExists()
        {
            // Arrange
            var repository = new EnclosureRepository();
            var enclosure = new Enclosure
            {
                Type = EnclosureType.forPredators,
                Size = new NoZeroPositiveInt(100),
                Capacity = new NoZeroPositiveInt(10),
                IsClean = true
            };

            await repository.AddAsync(enclosure);

            // Act
            var retrievedEnclosure = await repository.GetByIdAsync(enclosure.Id);

            // Assert
            Assert.NotNull(retrievedEnclosure);
            Assert.Equal(enclosure.Id, retrievedEnclosure.Id);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenEnclosureDoesNotExist()
        {
            // Arrange
            var repository = new EnclosureRepository();
            var nonExistentId = Guid.NewGuid();

            // Act
            var enclosure = await repository.GetByIdAsync(nonExistentId);

            // Assert
            Assert.Null(enclosure);
        }

        [Fact]
        public async Task GetEnclosureByAnimalIdAsync_ShouldReturnEnclosure_WhenAnimalExistsInEnclosure()
        {
            // Arrange
            var repository = new EnclosureRepository();
            var animalId = Guid.NewGuid();
            var enclosure = new Enclosure
            {
                Type = EnclosureType.forPredators,
                Size = new NoZeroPositiveInt(100),
                Capacity = new NoZeroPositiveInt(10),
                IsClean = true,
            };
            enclosure.AnimalIds.Add(animalId);

            await repository.AddAsync(enclosure);

            // Act
            var retrievedEnclosure = await repository.GetEnclosureByAnimalIdAsync(animalId);

            // Assert
            Assert.NotNull(retrievedEnclosure);
            Assert.Equal(enclosure.Id, retrievedEnclosure.Id);
        }

        [Fact]
        public async Task GetEnclosureByAnimalIdAsync_ShouldReturnNull_WhenAnimalDoesNotExistInAnyEnclosure()
        {
            // Arrange
            var repository = new EnclosureRepository();
            var animalId = Guid.NewGuid();

            // Act
            var enclosure = await repository.GetEnclosureByAnimalIdAsync(animalId);

            // Assert
            Assert.Null(enclosure);
        }

        [Fact]
        public async Task RemoveAsync_ShouldRemoveEnclosure()
        {
            // Arrange
            var repository = new EnclosureRepository();
            var enclosure = new Enclosure
            {
                Type = EnclosureType.forPredators,
                Size = new NoZeroPositiveInt(100),
                Capacity = new NoZeroPositiveInt(10),
                IsClean = true
            };

            await repository.AddAsync(enclosure);

            // Act
            await repository.RemoveAsync(enclosure.Id);
            var removedEnclosure = await repository.GetByIdAsync(enclosure.Id);

            // Assert
            Assert.Null(removedEnclosure);
        }
    }
}
