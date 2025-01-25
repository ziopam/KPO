using SecondKPOProject.Customers;

// ДОМАШНЕЕ ЗАДАНИЕ ВЫПОЛНЕНО ПО СТАРОМУ ЗАДАНИЮ

namespace SecondKPOProject
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Create a warehouse and factories
            CarWarehouse warehouse = new();
            PedalCarFactory pedalFactory = new(warehouse);
            HandCarFactory handCarFactory = new(warehouse);

            // Create a storage for customers
            CustomerStorage customerStorage = new();

            // Create a car shop
            HseCarShop carShop = new(warehouse, customerStorage);

            // Add customers to the storage
            customerStorage.AddCustomer(new Customer { Name = "Ivan", HandsPower = 3, LegsPower = 7 });
            customerStorage.AddCustomer(new Customer { Name = "Petr", HandsPower = 10, LegsPower = 10 });
            customerStorage.AddCustomer(new Customer { Name = "Sergey", HandsPower = 2, LegsPower = 2 });
            customerStorage.AddCustomer(new Customer { Name = "Dmitry", HandsPower = 5, LegsPower = 5 });

            // Create cars
            pedalFactory.CreateCarForReceiver();
            handCarFactory.CreateCarForReceiver();
            handCarFactory.CreateCarForReceiver();

            Console.WriteLine("-----Before sales:-----");

            // Print customers
            Console.WriteLine("Customers:");
            customerStorage.PrintCustomers();

            // Print cars
            Console.WriteLine();
            Console.WriteLine("Cars: ");
            warehouse.PrintCars();


            // Sale cars
            Console.WriteLine();
            Console.WriteLine("-----Saling carss:-----");
            int customersCount = customerStorage.CustomersCount;
            for (int i = 0; i < customersCount; i++)
            {
                carShop.SaleCar();
            }

            Console.WriteLine();
            Console.WriteLine("-----After sales:-----");
            Console.WriteLine("No customers in the storage. No cars in the warehouse.");

            // Print customers
            Console.WriteLine("Customers:");
            customerStorage.PrintCustomers();

            // Print cars
            Console.WriteLine();
            Console.WriteLine("Cars: ");
            warehouse.PrintCars();
        }
    }
}
