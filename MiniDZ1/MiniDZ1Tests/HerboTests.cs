using MiniDZ1.Animals;
using Moq;
using System.Reflection;
using String = System.String;

namespace MiniDZ1Tests
{
    public class HerboTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(5)]
        [InlineData(10)]
        public void HerboConstructor_ValidKindness_CreatesInstance(uint kindness)
        {
            try
            {
                var herbo = new Mock<Herbo>("", 0, 0, kindness).Object;
            }
            catch (ArgumentException)
            {
                Assert.Fail("Herbo constructor should not throw an exception with valid kindness");
            }
        }

        [Theory]
        [InlineData(11)]
        [InlineData(100)]
        [InlineData(100789)]
        public void HerboConstructor_InvalidKindness_ThrowsArgumentException(uint kindness)
        {
            var exception = Assert.Throws<TargetInvocationException>(() =>
            {
                var mock = new Mock<Herbo>("", 0, 0, kindness).Object;
            });

            Assert.IsType<ArgumentException>(exception.InnerException);
        }

        [Theory]
        [InlineData("Джейсон", 3, 2, 1, 0)]
        [InlineData("Зайцович", 5, 4, 6, 0)]
        [InlineData("Свен", 7, 6, 10, 0)]
        public void ToString_ReturnsCorrectString(String nickname, int food, int health, uint kindness, int number)
        {
            Herbo herbo = new Mock<Herbo>(nickname, food, health, kindness) { CallBase = true }.Object;
            herbo.Number = number;

            String herboString = herbo.ToString();

            Assert.Equal($" {nickname} (#{number}) с добротой {kindness} потребляет {food} кг в сутки", herboString);
        }

    }
}