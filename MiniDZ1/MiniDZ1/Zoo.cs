using MiniDZ1.Animals;
using MiniDZ1.HealthCheckers;
using MiniDZ1.Things;

namespace MiniDZ1
{
    internal class Zoo(IAnimalHealthChecker healthChecker)
    {
        private readonly IAnimalHealthChecker _healthChecker = healthChecker;
        private readonly List<Animal> _animals = [];
        private readonly List<Thing> _inventory = [];
        internal int NextNumber { get; private set; } = 1;

        public void AddAnimal(Animal animal)
        {
            if (_healthChecker.IsHealthy(animal))
            {
                animal.Number = NextNumber++;
                _animals.Add(animal);
                Console.WriteLine($"{animal.Nickname} (#{animal.Number}) принят в зоопарк.");
            }
            else
            {
                Console.WriteLine($"{animal.Nickname} не прошел осмотр. Он не принимается в зоопарк.");
            }
        }

        public void AddThing(Thing thing)
        {
            thing.Number = NextNumber++;
            _inventory.Add(thing);
            Console.WriteLine($"{thing.ThingName} (#{thing.Number}) добавлен на инвентаризацию.");
        }

        public int GetTotalFoodConsumption()
        {
            return _animals.Sum(a => a.Food);
        }

        public List<Animal> GetAllAnimals()
        {
            return [.. _animals];
        }

        public List<Herbo> GetInteractiveAnimals()
        {
            return _animals.OfType<Herbo>().Where(a => a.IsInterecative()).ToList();
        }

        public List<Thing> GetInventory()
        {
            return [.. _inventory];
        }

        public List<Predator> GetPredators()
        {
            return _animals.OfType<Predator>().ToList();
        }

        public List<Herbo> GetHerbos()
        {
            return _animals.OfType<Herbo>().ToList();
        }

    }
}
