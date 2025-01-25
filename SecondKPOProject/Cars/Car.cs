namespace SecondKPOProject;
/// <summary>
/// Car class.
/// </summary>
/// <param name="engineType">Type of engine</param>
internal class Car(EngineType engineType)
{
    private static int _amountOfCars = 0;
    public readonly int Number = ++_amountOfCars;
    public IEngine Engine { get; init; } = EngineFactory.CreateEngine(engineType);

    /// <summary>
    /// Returns a string representation of the car.
    /// </summary>
    /// <returns>String representation of the car.</returns>
    public override string ToString()
    {
        return $"Car with {Engine.EngineType} with number = {Number}";
    }

    /// <summary>
    /// Checks if the customer can use the car.
    /// </summary>
    /// <param name="customer">Customer object</param>
    /// <returns>True if customer can use the car, false otherwise.</returns>
    public bool CanCustomerUserIt(Customer customer)
    {
        if (Engine.EngineType == EngineType.HandControl)
        {
            return Engine.CanPersonUseIt(customer.HandsPower);
        }
        else
        {
            return Engine.CanPersonUseIt(customer.LegsPower);
        }
    }
}

