using Microsoft.Extensions.DependencyInjection;
using MiniDZ1.Animals;
using MiniDZ1.HealthCheckers;
using MiniDZ1.Things;

namespace MiniDZ1
{
    internal class Program
    {
        static void Main()
        {
            var serviceProvider = new ServiceCollection()
                .AddTransient<IAnimalHealthChecker, VeterinaryClinic>()
                .AddTransient<Zoo>()
                .BuildServiceProvider();

            var zoo = serviceProvider.GetRequiredService<Zoo>();

            Console.WriteLine("----Добавление животных и вещей в зоопарк----");
            // Herbivores.
            zoo.AddAnimal(new Monkey("Бошмачок", 2, 7, 10)); // Healthy and kind.
            zoo.AddAnimal(new Rabbit("Роджерс", 1, 8, 6)); // Healthy and kind.
            zoo.AddAnimal(new Monkey("Шимпанзе", 3, 2, 3)); // Not healthy and not kind.
            zoo.AddAnimal(new Rabbit("Багз", 3, 4, 5)); // Not healthy but kind.
            zoo.AddAnimal(new Rabbit("Лола", 2, 6, 4)); // Healthy and not kind enough.
            Console.WriteLine();


            // Predators (last parameter is actually not used, it's just for the difference between animals).
            zoo.AddAnimal(new Tiger("Тигруля", 5, 10, 1)); // Healthy.
            zoo.AddAnimal(new Tiger("Тигр", 6, 3, 2)); // Not healthy.
            zoo.AddAnimal(new Wolf("Волчара", 7, 7, 3)); // Healthy.
            zoo.AddAnimal(new Wolf("Вульф", 5, 4, 4)); // Not healthy.
            Console.WriteLine();

            // Things.
            zoo.AddThing(new Table());
            zoo.AddThing(new Computer());
            zoo.AddThing(new Computer());
            zoo.AddThing(new Table());
            zoo.AddThing(new Computer());
            zoo.AddThing(new Table());
            Console.WriteLine();

            // Display all the information.
            Console.WriteLine("----Информация о зоопарке----");
            Console.WriteLine($"Общее потребление еды: {zoo.GetTotalFoodConsumption()} кг/день");

            Console.WriteLine("\nСписок животных:");
            foreach (var animal in zoo.GetAllAnimals())
            {
                Console.WriteLine(animal);
            }

            Console.WriteLine("\nЖивотные для контактного зоопарка:");
            foreach (var animal in zoo.GetInteractiveAnimals())
            {
                Console.WriteLine(animal);
            }

            Console.WriteLine("\nТравоядные животные:");
            foreach (var herbivore in zoo.GetHerbos())
            {
                Console.WriteLine(herbivore);
            }


            Console.WriteLine("\nХищные животные:");
            foreach (var predator in zoo.GetPredators())
            {
                Console.WriteLine(predator);
            }

            Console.WriteLine("\nИнвентарь зоопарка (вещи):");
            foreach (var thing in zoo.GetInventory())
            {
                Console.WriteLine(thing);
            }

        }
    }
}
