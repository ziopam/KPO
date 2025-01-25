namespace SecondKPOProject;

internal class HseCarShop
{
    private readonly ICarSupplier _carSupplier;
    private readonly ICustomerSupplier _customerSupplier;

    public HseCarShop(ICarSupplier carSupplier, ICustomerSupplier customerSupplier)
    {
        if (carSupplier == null || customerSupplier == null)
        {
            throw new ArgumentNullException(nameof(carSupplier) + " or " + nameof(customerSupplier) + " is null");
        }
        _carSupplier = carSupplier;
        _customerSupplier = customerSupplier;
    }

    /// <summary>
    /// Sells a car to a customer. Car and customer are supplied by the suppliers.
    /// </summary>
    public void SaleCar()
    {
        var customer = _customerSupplier.SupplyCustomer();
        var car = _carSupplier.SupplyCar();

        if (customer == null || car == null)
        {
            Console.WriteLine("No customers or cars");
            return;
        }

        if (car.CanCustomerUserIt(customer) == false)
        {
            Console.WriteLine($"{customer.Name} can't use {car}. Car and customer leave the queue.");
            return;
        }

        Console.WriteLine($"{customer.Name} bought {car}");
        customer.Car = car;
    }
}
