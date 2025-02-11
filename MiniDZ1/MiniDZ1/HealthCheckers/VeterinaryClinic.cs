using MiniDZ1.Animals;

namespace MiniDZ1.HealthCheckers
{
    internal class VeterinaryClinic : IAnimalHealthChecker
    {
        public bool IsHealthy(Animal animal)
        {
            return animal.Health > 5;
        }
    }
}
