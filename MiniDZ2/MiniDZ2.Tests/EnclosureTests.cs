using MiniDZ2.Domain.Entities;
using MiniDZ2.Domain.ValueObjects;

namespace MiniDZ2.Tests
{
    public class EnclosureTests
    {
        [Fact]
        public void Enclosure_ShouldCreateEnclosure()
        {
            // Arrange & Act
            Enclosure enclosure = new()
            {
                Type = EnclosureType.forPredators,
                Size = new NoZeroPositiveInt(100),
                Capacity = new NoZeroPositiveInt(10)
            };

            // Assert
            Assert.NotNull(enclosure);
            Assert.Equal(EnclosureType.forPredators, enclosure.Type);
            Assert.Equal(100, enclosure.Size.Value);
            Assert.Equal(10, enclosure.Capacity.Value);
            Assert.True(enclosure.IsClean);
            Assert.Empty(enclosure.AnimalIds);
        }

        [Fact]
        public void Enclosure_AddAnimal_ShouldAddAnimal()
        {
            // Arrange
            Enclosure enclosure = new()
            {
                Type = EnclosureType.forPredators,
                Size = new NoZeroPositiveInt(100),
                Capacity = new NoZeroPositiveInt(10)
            };

            Animal animal = new()
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
            enclosure.AddAnimal(animal);

            // Assert
            Assert.Contains(animal.Id, enclosure.AnimalIds);
            Assert.Equal(1, enclosure.AmountOfAnimals);
        }

        [Fact]
        public void Enclosure_AddAnimal_ShouldThrowException_WhenEnclosureIsFull()
        {
            // Arrange
            Enclosure enclosure = new()
            {
                Type = EnclosureType.forPredators,
                Size = new NoZeroPositiveInt(100),
                Capacity = new NoZeroPositiveInt(1)
            };

            Animal animal1 = new()
            {
                Species = Species.Predator,
                Name = new AnimalName("Lion"),
                BirthDate = new BirthDate("01-01-2010"),
                Gender = Gender.Male,
                FavoriteFood = new Food("Meat"),
                Status = Status.Healthy,
                IsHungry = true
            };

            Animal animal2 = new()
            {
                Species = Species.Predator,
                Name = new AnimalName("Tiger"),
                BirthDate = new BirthDate("01-01-2011"),
                Gender = Gender.Female,
                FavoriteFood = new Food("Meat"),
                Status = Status.Healthy,
                IsHungry = true
            };

            // Act
            enclosure.AddAnimal(animal1);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => enclosure.AddAnimal(animal2));
        }

        [Fact]
        public void Enclosure_AddAnimal_ShouldThrowException_WhenEnclosureIsNotSuitable()
        {
            // Arrange
            Enclosure enclosure = new()
            {
                Type = EnclosureType.forHerbivores,
                Size = new NoZeroPositiveInt(100),
                Capacity = new NoZeroPositiveInt(10)
            };

            Animal animal = new()
            {
                Species = Species.Predator,
                Name = new AnimalName("Lion"),
                BirthDate = new BirthDate("01-01-2010"),
                Gender = Gender.Male,
                FavoriteFood = new Food("Meat"),
                Status = Status.Healthy,
                IsHungry = true
            };

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => enclosure.AddAnimal(animal));
        }

        [Fact]
        public void Enclosure_RemoveAnimal_ShouldRemoveAnimal()
        {
            // Arrange
            Enclosure enclosure = new()
            {
                Type = EnclosureType.forPredators,
                Size = new NoZeroPositiveInt(100),
                Capacity = new NoZeroPositiveInt(10)
            };

            Animal animal = new()
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
            enclosure.AddAnimal(animal);
            enclosure.RemoveAnimal(animal.Id);

            // Assert
            Assert.DoesNotContain(animal.Id, enclosure.AnimalIds);
            Assert.Equal(0, enclosure.AmountOfAnimals);
        }

        [Fact]
        public void Enclosure_HasAvaluablePlace_ShouldReturnTrue_WhenThereIsSpace()
        {
            // Arrange
            Enclosure enclosure = new()
            {
                Type = EnclosureType.forPredators,
                Size = new NoZeroPositiveInt(100),
                Capacity = new NoZeroPositiveInt(10)
            };

            // Act & Assert
            Assert.True(enclosure.HasAvaluablePlace());
        }

        [Fact]
        public void Enclosure_HasAvaluablePlace_ShouldReturnFalse_WhenThereIsNoSpace()
        {
            // Arrange
            Enclosure enclosure = new()
            {
                Type = EnclosureType.forPredators,
                Size = new NoZeroPositiveInt(100),
                Capacity = new NoZeroPositiveInt(1)
            };

            Animal animal = new()
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
            enclosure.AddAnimal(animal);

            // Assert
            Assert.False(enclosure.HasAvaluablePlace());
        }

        [Fact]
        public void Enclosure_Clean_ShouldSetIsCleanToTrue()
        {
            // Arrange
            Enclosure enclosure = new()
            {
                Type = EnclosureType.forPredators,
                Size = new NoZeroPositiveInt(100),
                Capacity = new NoZeroPositiveInt(10),
                IsClean = false
            };

            // Act
            enclosure.Clean();

            // Assert
            Assert.True(enclosure.IsClean);
        }

        [Fact]
        public void Enclosure_MarkAsDirty_ShouldSetIsCleanToFalse()
        {
            // Arrange
            Enclosure enclosure = new()
            {
                Type = EnclosureType.forPredators,
                Size = new NoZeroPositiveInt(100),
                Capacity = new NoZeroPositiveInt(10)
            };

            // Act
            enclosure.MarkAsDirty();

            // Assert
            Assert.False(enclosure.IsClean);
        }

        [Fact]
        public void Enclosure_IsEnclosureSuitable_ShouldReturnTrue_WhenEnclosureIsSuitable()
        {
            // Arrange
            Enclosure enclosure = new()
            {
                Type = EnclosureType.forPredators,
                Size = new NoZeroPositiveInt(100),
                Capacity = new NoZeroPositiveInt(10)
            };

            Animal animal = new()
            {
                Species = Species.Predator,
                Name = new AnimalName("Lion"),
                BirthDate = new BirthDate("01-01-2010"),
                Gender = Gender.Male,
                FavoriteFood = new Food("Meat"),
                Status = Status.Healthy,
                IsHungry = true
            };

            // Act & Assert
            Assert.True(enclosure.IsEnclosureSuitable(animal));
        }

        [Fact]
        public void Enclosure_IsEnclosureSuitable_ShouldReturnFalse_WhenEnclosureIsNotSuitable()
        {
            // Arrange
            Enclosure enclosure = new()
            {
                Type = EnclosureType.forHerbivores,
                Size = new NoZeroPositiveInt(100),
                Capacity = new NoZeroPositiveInt(10)
            };

            Animal animal = new()
            {
                Species = Species.Predator,
                Name = new AnimalName("Lion"),
                BirthDate = new BirthDate("01-01-2010"),
                Gender = Gender.Male,
                FavoriteFood = new Food("Meat"),
                Status = Status.Healthy,
                IsHungry = true
            };

            // Act & Assert
            Assert.False(enclosure.IsEnclosureSuitable(animal));
        }
    }
}
