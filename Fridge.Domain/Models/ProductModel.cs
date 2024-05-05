namespace Fridge.Domain.Models
{
    public class ProductModel
    {
        public long ProductId { get; set; }
        public required string Name { get; set; }
        public double Count { get; set; }
        public double Price { get; set; }
    }
}
