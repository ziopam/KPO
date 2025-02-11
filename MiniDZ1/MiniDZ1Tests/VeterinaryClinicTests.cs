using MiniDZ1.Animals;
using MiniDZ1.HealthCheckers;
using Moq;

namespace MiniDZ1Tests
{
    public class VeterinaryClinicTests
    {
        [Theory]
        [InlineData(6)]
        [InlineData(100)]
        [InlineData(100789)]
        public void IsHealthy_HealthGreaterThan5_ReturnsTrue(int health)
        {
            // Arrange
            var animal = new Mock<Animal>("", 0, health);
            var veterinaryClinic = new VeterinaryClinic();

            // Act
            var result = veterinaryClinic.IsHealthy(animal.Object);

            // Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-5)]
        [InlineData(5)]
        public void IsHealthy_HealthLessOrEqualTo5_ReturnsFalse(int health)
        {
            // Arrange
            var animal = new Mock<Animal>("", 0, health);
            var veterinaryClinic = new VeterinaryClinic();

            // Act
            var result = veterinaryClinic.IsHealthy(animal.Object);

            // Assert
            Assert.False(result);
        }
    }
}
