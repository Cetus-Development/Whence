namespace Whence.Samples.CustomerApp
{
    public class FilterSettings
    {
        private FilterMode filter;
        public FilterMode Filter
        {
            get => filter;
            set
            {
                if (value != filter)
                {
                    filter = value;
                    FilterChanged?.Invoke(value);
                }
            }

        }

        public event Action<FilterMode>? FilterChanged;
    }
}