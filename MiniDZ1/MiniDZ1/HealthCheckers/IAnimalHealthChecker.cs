using MiniDZ1.Animals;

namespace MiniDZ1.HealthCheckers
{
    internal interface IAnimalHealthChecker
    {
        public bool IsHealthy(Animal alive);
    }
}
