namespace SecondKPOProject.Customers
{
    internal class CustomerStorage : ICustomerAddable, ICustomerSupplier
    {
        private readonly List<Customer> _customers = new();
        public int CustomersCount => _customers.Count;

        public void AddCustomer(Customer customer)
        {
            _customers.Add(customer);
        }

        public Customer? SupplyCustomer()
        {
            if (_customers.Count == 0)
            {
                Console.WriteLine("No customers");
                return null;
            }

            var customer = _customers.First();
            _customers.Remove(customer);
            return customer;
        }

        public void PrintCustomers()
        {
            foreach (var customer in _customers)
            {
                Console.WriteLine(customer);
            }
        }
    }
}
