using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiEcommerce.Models.Dtos
{
    public class UpdateProductDto
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public required string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Range(0, double.MaxValue)]
        [Column(TypeName = "decimal(18,2)")]
        public required decimal Price { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public required string SKU { get; set; } = string.Empty;
        [Range(0, int.MaxValue)]
        public int Stock { get; set; }
        public int CategoryId { get; set; }
    }
}
