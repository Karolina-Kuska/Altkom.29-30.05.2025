using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Order : Entity
    {
        // 3 na nazwę backing field dla właściwości, które są rozpoznawane przez wbudowane konwencje EF Core
        //private string name = string.Empty;
        //private string _name = string.Empty;
        //private string m_name = string.Empty;


        private string orderName = string.Empty;

        public string Name
        {
            get => orderName;
            set
            {
                orderName = value;
                OnPropertyChanged();
            }
        }

        //odpowiednik IsConcurrencyToken w konfiguracji 
        //[ConcurrencyCheck]
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public virtual ICollection<Product> Products { get; set; } = new ObservableCollection<Product>();

        public float Value { get; set; }
        public float Tax { get; set; }

        //public float TotalValue => Value * (1 + Tax);
        public float TotalValue { get; }
        //public bool IsExpired { get; }

        public OrderType OrderType { get; set; }
        public Parameters Parameters { get; set; }
    }
}
