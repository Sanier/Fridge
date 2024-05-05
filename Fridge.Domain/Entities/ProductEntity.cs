using System.ComponentModel.DataAnnotations;

namespace Fridge.Domain.Entities
{
    public class ProductEntity
    {
        [Key]
        public long ProductId { get; set; }
        public required string Name { get; set; }
        public double Count { get; set; }
        public double Price { get; set; }
    }
}
