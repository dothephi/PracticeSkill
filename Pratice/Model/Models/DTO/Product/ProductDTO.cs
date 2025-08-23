using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Models.DTO.Product
{
    public class ProductDTO
    {
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
    }
}
