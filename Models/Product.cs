using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Product : Entity
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; } = 0.0m;
        public Order Order { get; set; }

        //odpowiednik IsRowVersion w konfiguracji
        //[Timestamp]
        public byte[] Timestamp { get; }
    }
}