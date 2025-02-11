using MiniDZ1.Things;
using Moq;
using String = System.String;

namespace MiniDZ1Tests
{
    public class ThingTests
    {
        [Theory]
        [InlineData(3)]
        [InlineData(2)]
        [InlineData(6)]
        public void ToString_ReturnsCorrectString(int number)
        {
            Thing thing = new Mock<Thing>() { CallBase = true }.Object;
            thing.Number = number;

            String thingString = thing.ToString();

            Assert.Equal($" #{number}", thingString);
        }
    }
}
