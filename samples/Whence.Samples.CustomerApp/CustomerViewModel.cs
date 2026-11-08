using System.Collections.ObjectModel;

namespace Whence.Samples.CustomerApp
{
    public class CustomerViewModel
    {
        private ICustomerRepository customerRepository;
        private FilterSettings filterSettings;
        public ObservableCollection<Customer> Customers { get; set; }


        public CustomerViewModel(ICustomerRepository _customerrepository, FilterSettings _settings)
        {
            customerRepository = _customerrepository;
            filterSettings = _settings;
            Customers = new ObservableCollection<Customer>();

            _settings.FilterChanged += OnFilterChanged;
        }

        private void OnFilterChanged(FilterMode _mode)
        {
            Reload();
        }

        public void Reload()
        {
            Customers.Clear();
            ApplyFilter(filterSettings.Filter);
        }

        public void ApplyFilter(FilterMode _mode)
        {
            switch (_mode)
            {
                case FilterMode.All:
                    customerRepository.ForEach(c => Customers.Add(c));
                    break;

                case FilterMode.ActiveOnly:
                case FilterMode.InactiveOnly:
                    bool wantActive = _mode == FilterMode.ActiveOnly;

                    foreach (var customer in customerRepository.GetAll().OrderBy(c => c.Name))
                    {
                        if (customer.IsActive == wantActive)
                        {
                            Customers.Add(customer);
                        }
                    }
                    break;

            }
        }

    }
}