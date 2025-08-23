using System.ComponentModel.DataAnnotations;

namespace Model.Models
{
    public class Products
    {
        [Key]
        public string ProductId { get; set; }

        public string Name { get; set; }

        public int Quantity { get; set; }

        public string Quality { get; set; }

        public int? Stock { get; set; }

        public string? Suplier { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public string? A { get; set; }

        public string? B { get; set; }

        public string? C { get; set; }

        public string AccountId { get; set; }
        public virtual SystemUserAccount SystemUserAccount { get; set; }
    }
}
