using MiniDZ1.Animals;
using Moq;
using System.Reflection;
using String = System.String;

namespace MiniDZ1Tests
{
    public class PredatorTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(5)]
        [InlineData(10)]
        public void PredatorConstructor_ValidAggressiveness_CreatesInstance(uint aggressiveness)
        {
            try
            {
                var predator = new Mock<Predator>("", 0, 0, aggressiveness).Object;
            }
            catch (ArgumentException)
            {
                Assert.Fail("Predator constructor should not throw an exception with valid aggressiveness");
            }
        }

        [Theory]
        [InlineData(11)]
        [InlineData(100)]
        [InlineData(100789)]
        public void PredatorConstructor_InvalidAggressiveness_ThrowsArgumentException(uint aggressiveness)
        {
            var exception = Assert.Throws<TargetInvocationException>(() =>
            {
                var mock = new Mock<Predator>("", 0, 0, aggressiveness).Object;
            });

            Assert.IsType<ArgumentException>(exception.InnerException);
        }

        [Theory]
        [InlineData("Лев", 3, 2, 1, 0)]
        [InlineData("Тигр", 5, 4, 6, 0)]
        [InlineData("Волк", 7, 6, 10, 0)]
        public void ToString_ReturnsCorrectString(String nickname, int food, int health, uint aggressiveness, int number)
        {
            Predator predator = new Mock<Predator>(nickname, food, health, aggressiveness) { CallBase = true }.Object;
            predator.Number = number;

            String predatorString = predator.ToString();

            Assert.Equal($" {nickname} (#{number}) с агрессивностью {aggressiveness} потребляет {food} кг в сутки", predatorString);
        }
    }
}
