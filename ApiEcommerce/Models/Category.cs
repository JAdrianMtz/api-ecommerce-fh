using System.ComponentModel.DataAnnotations;

namespace ApiEcommerce.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public required string Name { get; set; } = string.Empty;
        [Required]
        public DateTime CreatedAt { get; set; }
    }
}
