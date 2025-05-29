using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Order : Entity
    {
        private string _name = string.Empty;

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        //odpowiednik IsConcurrencyToken w konfiguracji 
        //[ConcurrencyCheck]
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public ICollection<Product> Products { get; set; } = new ObservableCollection<Product>();
    }
}
