using System.ComponentModel.DataAnnotations;

namespace ApiEcommerce.Models.Dtos
{
    public class CreateCategoryDto
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [MaxLength(50, ErrorMessage = "El campo {0} debe tener un máximo de {1} caracteres")]
        [MinLength(3, ErrorMessage = "El campo {0} debe tener un mínimo de {1} caracteres")]
        public string Name { get; set; } = string.Empty;
    }
}
