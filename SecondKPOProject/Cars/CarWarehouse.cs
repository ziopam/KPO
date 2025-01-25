namespace SecondKPOProject
{
    internal class CarWarehouse : ICarReceiver, ICarSupplier
    {
        private readonly List<Car> _cars = new();

        /// <summary>
        /// Adds a car to the warehouse. If car is null, does nothing.
        /// </summary>
        /// <param name="car">Car to add in ware house.</param>
        public void ReceiveCar(Car car)
        {
            if (car != null)
            {
                _cars.Add(car);
            }
        }

        /// <summary>
        /// Supplies a car.
        /// </summary>
        /// <returns>A car object. Returns null if ware house is empty.</returns>
        public Car? SupplyCar()
        {
            if (_cars.Count == 0)
            {
                return null;
            }

            var car = _cars.First();
            _cars.Remove(car);
            return car;
        }

        /// <summary>
        /// Prints all cars in the warehouse.
        /// </summary>
        public void PrintCars()
        {
            foreach (var car in _cars)
            {
                Console.WriteLine(car);
            }
        }
    }
}
