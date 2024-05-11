using System.ComponentModel.DataAnnotations;

namespace Fridge.Domain.Models
{
    public class ProductModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public double Count { get; set; }

        [Required]
        public double Price { get; set; }
    }
}
