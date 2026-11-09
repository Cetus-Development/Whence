namespace Whence.Samples.CustomerApp
{
    public class CustomerRepository : ICustomerRepository
    {
        private List<Customer> customers = new List<Customer>
        {
            new Customer("Andrew", true),
            new Customer("Lily", true),
            new Customer("Arthur", false),
            new Customer("Robin", true),
            new Customer("Mathilda", false),
            new Customer("Maja", false),
        };

        public IEnumerable<Customer> GetAll() => customers;

        // Deliberately uses a callback: Whence must detect this relationship type.
        public void ForEach(Action<Customer> action)
        {
            foreach (var customer in customers)
            {
                action(customer);
            }
        }

        public async Task<IEnumerable<Customer>> GetCustomersAsync()
        {
            await Task.Delay(1000);
            List<Customer> newList = new List<Customer>
            {
                new Customer("Hans", false),
                new Customer("Nadia", true)
            };

            return newList;
        }

    }
}