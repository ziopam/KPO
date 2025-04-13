using MiniDZ2.Domain.Entities;
using MiniDZ2.Domain.ValueObjects;
using MiniDZ2.Infrastructure.Repositories;

namespace MiniDZ2.Tests
{
    public class AnimalRepositoryTests
    {
        [Fact]
        public async Task AddAsync_ShouldAddAnimal()
        {
            // Arrange
            var repository = new AnimalRepository();
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

            // Act
            var result = await repository.AddAsync(animal);

            // Assert
            Assert.True(result);
            var addedAnimal = await repository.GetByIdAsync(animal.Id);
            Assert.NotNull(addedAnimal);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllAnimals()
        {
            // Arrange
            var repository = new AnimalRepository();
            var animal1 = new Animal
            {
                Species = Species.Predator,
                Name = new AnimalName("Lion"),
                BirthDate = new BirthDate("01-01-2010"),
                Gender = Gender.Male,
                FavoriteFood = new Food("Meat"),
                Status = Status.Healthy,
                IsHungry = true
            };
            var animal2 = new Animal
            {
                Species = Species.Predator,
                Name = new AnimalName("Tiger"),
                BirthDate = new BirthDate("01-01-2011"),
                Gender = Gender.Female,
                FavoriteFood = new Food("Meat"),
                Status = Status.Healthy,
                IsHungry = true
            };

            await repository.AddAsync(animal1);
            await repository.AddAsync(animal2);

            // Act
            var animals = await repository.GetAllAsync();

            // Assert
            Assert.Contains(animal1, animals);
            Assert.Contains(animal2, animals);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnAnimal_WhenAnimalExists()
        {
            // Arrange
            var repository = new AnimalRepository();
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

            await repository.AddAsync(animal);

            // Act
            var retrievedAnimal = await repository.GetByIdAsync(animal.Id);

            // Assert
            Assert.NotNull(retrievedAnimal);
            Assert.Equal(animal.Id, retrievedAnimal.Id);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenAnimalDoesNotExist()
        {
            // Arrange
            var repository = new AnimalRepository();
            var nonExistentId = Guid.NewGuid();

            // Act
            var animal = await repository.GetByIdAsync(nonExistentId);

            // Assert
            Assert.Null(animal);
        }

        [Fact]
        public async Task RemoveAsync_ShouldRemoveAnimal()
        {
            // Arrange
            var repository = new AnimalRepository();
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

            await repository.AddAsync(animal);

            // Act
            await repository.RemoveAsync(animal.Id);
            var removedAnimal = await repository.GetByIdAsync(animal.Id);

            // Assert
            Assert.Null(removedAnimal);
        }
    }
}
