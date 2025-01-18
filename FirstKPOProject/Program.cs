namespace FirstKPOProject
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var customers = new List<Customer>
            {
                new() {Name = "Olesya"},
                new() {Name = "Nikita"},
                new() {Name = "Alex"},
                new() {Name = "Jinx"}
            };

            var factory = new FactoryAF(customers);

            for (int i = 0; i < 3; i++)
            {
                factory.AddCar();
            }

            Console.WriteLine("Before");
            Console.WriteLine(factory);

            factory.SaleCar();
            Console.WriteLine("---------------------------");

            Console.WriteLine("After");
            Console.WriteLine(factory);
            Console.WriteLine("Car owners: \n");
            foreach (var customer in customers)
            {
                Console.WriteLine(customer);
            }
        }
    }
}
