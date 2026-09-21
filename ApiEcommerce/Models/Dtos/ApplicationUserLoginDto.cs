using System.ComponentModel.DataAnnotations;

namespace ApiEcommerce.Models.Dtos
{
    public class ApplicationUserLoginDto
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public required string Email { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public required string Password { get; set; }
    }
}
