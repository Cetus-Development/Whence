using Whence.Samples.CustomerApp;

CustomerRepository customerRepository = new CustomerRepository();
FilterSettings filterSettings = new FilterSettings();
CustomerViewModel customerVM = new CustomerViewModel(customerRepository, filterSettings);

customerVM.Reload();

filterSettings.Filter = FilterMode.ActiveOnly;
filterSettings.Filter = FilterMode.InactiveOnly;

await customerVM.LoadMoreAsync();

foreach (Customer customer in customerVM.Customers)
{
    System.Console.WriteLine("Name: " + customer.Name + ", Active: " + customer.IsActive);
}
