using MiniDZ2.Domain.Entities;
using MiniDZ2.Domain.ValueObjects;

namespace MiniDZ2.Tests
{
    public class AnimalTests
    {

        [Fact]
        public void Animal_ShouldCreateAnimal()
        {
            // Arrange & act
            Animal animal = new()
            {
                Species = Species.Herbivore,
                Name = new AnimalName("Тест"),
                BirthDate = new BirthDate("10-10-2010"),
                Gender = Gender.Male,
                FavoriteFood = new Food("Тест"),
                Status = Status.Healthy,
                IsHungry = true,
            };

            // Assert
            Assert.NotNull(animal);
            Assert.Equal("Тест", animal.Name.Value);
            Assert.Equal(new DateOnly(2010, 10, 10), animal.BirthDate.Value);
            Assert.Equal("Самец", animal.Gender.Value);
            Assert.Equal("Тест", animal.FavoriteFood.Value);
            Assert.Equal("Здоров", animal.Status.Value);
            Assert.Equal(Guid.Empty, animal.EnclosureId);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Animal_ShouldThrowException_WhenStringIsEmpty(string? str)
        {
            // Arrange & act & assert
            Assert.Throws<ArgumentException>(() => new AnimalName(str!));
            Assert.Throws<ArgumentException>(() => new Food(str!));
            Assert.Throws<ArgumentException>(() => new BirthDate(str!));
        }

        [Fact]
        public void BirthDate_ShouldThrowException_WhenDateIsInFuture()
        {
            // Arrange & act & assert
            Assert.Throws<ArgumentException>(() => new BirthDate(DateTime.Now.AddDays(1).ToString("dd-MM-yyyy")));
        }

        [Fact]
        public void BirthDate_ToString_ShouldReturnCorrectFormat()
        {
            // Arrange & act
            var birthDate = new BirthDate("01-01-2020");

            // Assert
            Assert.Equal("01-01-2020", birthDate.ToString());
        }

        [Fact]
        public void Gender_GetGenderByString_ReturnsGenderObjectsOrThrowsExecption()
        {
            // Arrange & act & assert
            Assert.Equal(Gender.GetGenderByString("Самец"), Gender.Male);
            Assert.Equal(Gender.GetGenderByString("Самка"), Gender.Female);
            Assert.Throws<ArgumentException>(() => Gender.GetGenderByString("dshfkjs"));
        }

        [Fact]
        public void Status_GetStatusByString_ReturnsStatusObjectsOrThrowsExecption()
        {
            // Arrange & act & assert
            Assert.Equal(Status.GetStatusByString("Здоров"), Status.Healthy);
            Assert.Equal(Status.GetStatusByString("Болен"), Status.Sick);
            Assert.Throws<ArgumentException>(() => Status.GetStatusByString("dshfkjs"));
        }

        [Fact]
        public void Species_GetSpeciesByString_ReturnsSpeciesObjectsOrThrowsExecption()
        {
            // Arrange & act & assert
            Assert.Equal(Species.GetSpeciesByString("Травоядное"), Species.Herbivore);
            Assert.Equal(Species.GetSpeciesByString("Хищник"), Species.Predator);
            Assert.Equal(Species.GetSpeciesByString("Рыба"), Species.Fish);
            Assert.Equal(Species.GetSpeciesByString("Птица"), Species.Bird);
            Assert.Throws<ArgumentException>(() => Species.GetSpeciesByString("dshfkjs"));
        }

        [Fact]
        public void Animal_MarkAsHungry_ShouldSetIsHungryToTrue()
        {
            // Arrange
            Animal animal = new()
            {
                Species = Species.Herbivore,
                Name = new AnimalName("Тест"),
                BirthDate = new BirthDate("10-10-2010"),
                Gender = Gender.Male,
                FavoriteFood = new Food("Тест"),
                Status = Status.Healthy,
                IsHungry = false
            };

            // Act
            animal.MarkAsHungry();

            // Assert
            Assert.True(animal.IsHungry);
        }

        [Fact]
        public void Animal_Feed_ShouldSetIsHungryToFalse()
        {
            // Arrange
            Animal animal = new()
            {
                Species = Species.Herbivore,
                Name = new AnimalName("Тест"),
                BirthDate = new BirthDate("10-10-2010"),
                Gender = Gender.Male,
                FavoriteFood = new Food("Тест"),
                Status = Status.Healthy,
                IsHungry = true
            };

            // Act
            animal.Feed();

            // Assert
            Assert.False(animal.IsHungry);
        }

        [Fact]
        public void Animal_MarkAsSick_ShouldSetStatusToSick()
        {
            // Arrange
            Animal animal = new()
            {
                Species = Species.Herbivore,
                Name = new AnimalName("Тест"),
                BirthDate = new BirthDate("10-10-2010"),
                Gender = Gender.Male,
                FavoriteFood = new Food("Тест"),
                Status = Status.Healthy,
                IsHungry = true
            };

            // Act
            animal.MarkAsSick();

            // Assert
            Assert.Equal(animal.Status, Status.Sick);
        }

        [Fact]
        public void Animal_Heal_ShouldSetStatusToHealthy()
        {
            // Arrange
            Animal animal = new()
            {
                Species = Species.Herbivore,
                Name = new AnimalName("Тест"),
                BirthDate = new BirthDate("10-10-2010"),
                Gender = Gender.Male,
                FavoriteFood = new Food("Тест"),
                Status = Status.Sick,
                IsHungry = true
            };

            // Act
            animal.Heal();

            // Assert
            Assert.Equal(animal.Status, Status.Healthy);
        }

        [Fact]
        public void Animal_MoveToEnclosure_ShouldChangeEnclosureId()
        {
            // Arrange
            Animal animal = new()
            {
                Species = Species.Herbivore,
                Name = new AnimalName("Тест"),
                BirthDate = new BirthDate("10-10-2010"),
                Gender = Gender.Male,
                FavoriteFood = new Food("Тест"),
                Status = Status.Sick,
                IsHungry = true
            };
            Guid enclosureGuid = Guid.NewGuid();

            // Act
            animal.MoveToEnclosure(enclosureGuid);

            // Assert
            Assert.Equal(animal.EnclosureId, enclosureGuid);
        }

        [Fact]
        public void Animal_IsInEnclosure_ShouldReturnFalse()
        {
            // Arrange
            Animal animal = new()
            {
                Species = Species.Herbivore,
                Name = new AnimalName("Тест"),
                BirthDate = new BirthDate("10-10-2010"),
                Gender = Gender.Male,
                FavoriteFood = new Food("Тест"),
                Status = Status.Sick,
                IsHungry = true
            };

            // Act & Assert
            Assert.False(animal.IsInEnclosure());
        }

        [Fact]
        public void Animal_IsInEnclosure_ShouldReturnTrue()
        {
            // Arrange
            Animal animal = new()
            {
                Species = Species.Herbivore,
                Name = new AnimalName("Тест"),
                BirthDate = new BirthDate("10-10-2010"),
                Gender = Gender.Male,
                FavoriteFood = new Food("Тест"),
                Status = Status.Sick,
                IsHungry = true
            };
            Guid enclosureGuid = Guid.NewGuid();

            // Act
            animal.MoveToEnclosure(enclosureGuid);

            // Assert
            Assert.True(animal.IsInEnclosure());
        }
    }
}
