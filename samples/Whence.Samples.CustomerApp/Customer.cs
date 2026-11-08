namespace Whence.Samples.CustomerApp
{
    public class Customer
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }

        public Customer(string _name, bool _isActive)
        {
            Name = _name;
            IsActive = _isActive;
        }
    }
}