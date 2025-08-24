using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Models.DTO.Product
{
    public class CreateProductDTO
    {
        public int ProductId { get; set; }

        public string Name { get; set; }

        public int Quantity { get; set; }

        public string Quality { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public string AccountId { get; set; }
    }
}
