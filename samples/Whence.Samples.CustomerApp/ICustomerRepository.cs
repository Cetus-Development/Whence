namespace Whence.Samples.CustomerApp
{
    public interface ICustomerRepository
    {
        IEnumerable<Customer> GetAll();
        void ForEach(Action<Customer> action);
    }
}