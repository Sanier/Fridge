using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Fridge.Domain.Models;

namespace Fridge.Domain.Entities
{
    public class ProductEntity
    {
        [Key]
        [StringLength(36)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ProductId { get; set; }

        [Required]
        public string Name { get; set; }
        public double Count { get; set; }
        public double Price { get; set; }

        public long UserId { get; set; }
        public UserEntity User { get; set; }
    }
}
