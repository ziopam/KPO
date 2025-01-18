
using System.Text;

namespace FirstKPOProject;

internal class FactoryAF(List<Customer> customers)
{
    public List<Car> Cars { get; private set; } = [];

    public List<Customer> Customers { get; private set; } = customers;

    public void AddCar()
    {
        var car = new Car { Number = Cars.Count + 1 };
        Cars.Add(car);
    }

    public void SaleCar()
    {
        foreach (var customer in Customers)
        {
            customer.Car ??= Cars.LastOrDefault();

            if (customer.Car == null)
            {
                break;
            }

            Cars.RemoveAt(Cars.Count - 1);
        }

        Customers = Customers.Where(customer => customer.Car == null).ToList();
        Cars.Clear();
    }

    public override string ToString()
    {
        StringBuilder sb = new();
        sb.Append($"Factory with {Cars.Count} cars and {Customers.Count} customers\n");

        sb.Append("\nCars:\n");
        foreach (var car in Cars)
        {
            sb.Append(car.ToString());
            sb.Append('\n');
        }

        sb.Append("\nCustomers:\n");
        foreach (var customer in Customers)
        {
            sb.Append(customer.ToString());
            sb.Append('\n');
        }

        return sb.ToString();
    }
}
