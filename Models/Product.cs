using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Product : Entity
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; } = 0.0m;

        //shadow property - właściwość, która nie jest mapowana do bazy danych, ale jest używana w kontekście EF Core
        //public int OrderId { get; set; }
        public Order Order { get; set; }

        //odpowiednik IsRowVersion w konfiguracji
        //[Timestamp]
        //public byte[] Timestamp { get; }
    }
}