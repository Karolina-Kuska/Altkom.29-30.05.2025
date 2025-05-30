using Microsoft.EntityFrameworkCore.Infrastructure;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Product : Entity
    {
        private ILazyLoader _lazyLoader;
        private Order? _order;

        public Product()
        {
        }
        public Product(ILazyLoader lazyLoader)
        {
            _lazyLoader = lazyLoader;
        }

        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; } = 0.0m;

        //shadow property - właściwość, która nie jest mapowana do bazy danych, ale jest używana w kontekście EF Core
        //public int OrderId { get; set; }
        public Order? Order
        {
            get
            {
                if(_order == null)
                {
                    try
                    {
                        _lazyLoader?.Load(this, ref _order);
                    }
                    catch
                    {
                        _order = null;
                    }
                }
                return _order;
            }

            set => _order = value;
        }

        //public virtual Order? Order { get; set; }

        //odpowiednik IsRowVersion w konfiguracji
        //[Timestamp]
        //public byte[] Timestamp { get; }


        /*public float Weight { get; set; }
        public float Height { get; set; }
        public float Width { get; set; }
        public float Depth { get; set; }*/
        public ProductDetails? ProductDetails { get; set; }
    }
}