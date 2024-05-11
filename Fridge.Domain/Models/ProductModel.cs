using System.ComponentModel.DataAnnotations;

namespace Fridge.Domain.Models
{
    public class ProductModel
    {
        public long ProductId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public double Count { get; set; }

        [Required]
        public double Price { get; set; }

        public long UserId { get; set; }
    }
}
