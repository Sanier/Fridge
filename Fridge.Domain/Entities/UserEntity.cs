using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Fridge.Domain.Models;

namespace Fridge.Domain.Entities
{
    public class UserEntity
    {
        [Key]
        [StringLength(36)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long UserId { get; set; }

        [StringLength(36)]
        public string Email { get; set; }

        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }

        public ICollection<ProductEntity> Products { get; set; }
    }
}
