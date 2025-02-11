using MiniDZ1;
using MiniDZ1.Animals;
using MiniDZ1.HealthCheckers;
using MiniDZ1.Things;
using Moq;

namespace MiniDZ1Tests
{
    public class ZooTests
    {
        [Fact]
        public void AddAnimal_HealthyAnimal_AddedToZoo()
        {
            // Arrange
            var healthCheckerMock = new Mock<IAnimalHealthChecker>();
            healthCheckerMock.Setup(hc => hc.IsHealthy(It.IsAny<Animal>())).Returns(true);

            var zoo = new Zoo(healthCheckerMock.Object);
            int nextNumber = zoo.NextNumber;

            Animal animal = new Mock<Animal>("", 0, 0).Object;

            // Act
            zoo.AddAnimal(animal);

            // Assert
            Assert.Equal(nextNumber, animal.Number);
            Assert.Equal(nextNumber + 1, zoo.NextNumber);
            Assert.Contains(animal, zoo.GetAllAnimals());
        }

        [Fact]
        public void AddAnimal_UnhealthyAnimal_NotAddedToZoo()
        {
            // Arrange
            var healthCheckerMock = new Mock<IAnimalHealthChecker>();
            healthCheckerMock.Setup(hc => hc.IsHealthy(It.IsAny<Animal>())).Returns(false);

            var zoo = new Zoo(healthCheckerMock.Object);
            int nextNumber = zoo.NextNumber;

            Animal animal = new Mock<Animal>("", 0, 0).Object;

            // Act
            zoo.AddAnimal(animal);

            // Assert
            Assert.Equal(-1, animal.Number);
            Assert.Equal(nextNumber, zoo.NextNumber);
            Assert.DoesNotContain(animal, zoo.GetAllAnimals());
        }

        [Fact]
        public void GetInteractiveAnimals_ReturnsInteractiveAnimals()
        {
            // Arrange
            var healthCheckerMock = new Mock<IAnimalHealthChecker>();
            healthCheckerMock.Setup(hc => hc.IsHealthy(It.IsAny<Animal>())).Returns(true);

            var zoo = new Zoo(healthCheckerMock.Object);

            var random_animal = new Mock<Animal>("", 2, 0).Object;
            var tiger = new Tiger("", 0, 0, 10);
            var wolf = new Wolf("", 0, 0, 10);
            var monkey = new Monkey("", 0, 0, 6);
            var rabbit = new Rabbit("", 0, 0, 5);

            zoo.AddAnimal(random_animal);
            zoo.AddAnimal(tiger);
            zoo.AddAnimal(wolf);
            zoo.AddAnimal(monkey);
            zoo.AddAnimal(rabbit);

            // Act
            List<Herbo> interactiveAnimals = zoo.GetInteractiveAnimals();

            // Assert
            Assert.Single(interactiveAnimals);
            Assert.Contains(monkey, interactiveAnimals);
        }

        [Fact]
        public void GetPredators_ReturnsPredators()
        {
            // Arrange
            var healthCheckerMock = new Mock<IAnimalHealthChecker>();
            healthCheckerMock.Setup(hc => hc.IsHealthy(It.IsAny<Animal>())).Returns(true);

            var zoo = new Zoo(healthCheckerMock.Object);

            var some_predator = new Mock<Predator>("", 0, 0, (uint)1).Object;
            var tiger = new Tiger("", 0, 0, 0);
            var wolf = new Wolf("", 0, 0, 0);
            var monkey = new Monkey("", 0, 0, 0);
            var rabbit = new Rabbit("", 0, 0, 0);

            zoo.AddAnimal(some_predator);
            zoo.AddAnimal(tiger);
            zoo.AddAnimal(wolf);
            zoo.AddAnimal(monkey);
            zoo.AddAnimal(rabbit);

            // Act
            List<Predator> predators = zoo.GetPredators();

            // Assert
            Assert.Equal(3, predators.Count);
            Assert.Contains(some_predator, predators);
            Assert.Contains(tiger, predators);
            Assert.Contains(wolf, predators);
        }

        [Fact]
        public void GetHerbos_ReturnsHerbos()
        {
            // Arrange
            var healthCheckerMock = new Mock<IAnimalHealthChecker>();
            healthCheckerMock.Setup(hc => hc.IsHealthy(It.IsAny<Animal>())).Returns(true);

            var zoo = new Zoo(healthCheckerMock.Object);

            var some_herbo = new Mock<Herbo>("", 0, 0, (uint)0).Object;
            var tiger = new Tiger("", 0, 0, 0);
            var wolf = new Wolf("", 0, 0, 0);
            var monkey = new Monkey("", 0, 0, 0);
            var rabbit = new Rabbit("", 0, 0, 0);

            zoo.AddAnimal(some_herbo);
            zoo.AddAnimal(tiger);
            zoo.AddAnimal(wolf);
            zoo.AddAnimal(monkey);
            zoo.AddAnimal(rabbit);

            // Act
            List<Herbo> herbos = zoo.GetHerbos();

            // Assert
            Assert.Equal(3, herbos.Count);
            Assert.Contains(some_herbo, herbos);
            Assert.Contains(monkey, herbos);
            Assert.Contains(rabbit, herbos);
        }

        [Fact]
        public void GetTotalFoodConsumption_ReturnsSumOfFood()
        {
            // Arrange
            var healthCheckerMock = new Mock<IAnimalHealthChecker>();
            healthCheckerMock.Setup(hc => hc.IsHealthy(It.IsAny<Animal>())).Returns(true);

            var zoo = new Zoo(healthCheckerMock.Object);
            zoo.AddAnimal(new Mock<Animal>("", 2, 0).Object);
            zoo.AddAnimal(new Tiger("", 10, 0, 0));
            zoo.AddAnimal(new Wolf("", 20, 0, 0));
            zoo.AddAnimal(new Monkey("", 52, 0, 0));
            zoo.AddAnimal(new Rabbit("", 1, 0, 0));

            // Act
            int totalFoodConsumption = zoo.GetTotalFoodConsumption();

            // Assert
            Assert.Equal(85, totalFoodConsumption);
        }

        [Fact]
        public void AddThing_ThingAddedToInventory()
        {
            // Arrange
            var healthCheckerMock = new Mock<IAnimalHealthChecker>();
            var zoo = new Zoo(healthCheckerMock.Object);
            int nextNumber = zoo.NextNumber;

            Thing thing = new Mock<Thing>().Object;
            Table table = new();
            Computer computer = new();


            // Act
            zoo.AddThing(thing);
            zoo.AddThing(table);
            zoo.AddThing(computer);

            // Assert
            Assert.Equal(nextNumber, thing.Number);
            Assert.Equal(nextNumber + 1, table.Number);
            Assert.Equal(nextNumber + 2, computer.Number);
            Assert.Equal(nextNumber + 3, zoo.NextNumber);
            Assert.Contains(thing, zoo.GetInventory());
            Assert.Contains(table, zoo.GetInventory());
            Assert.Contains(computer, zoo.GetInventory());
        }
    }
}