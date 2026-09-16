using System.ComponentModel.DataAnnotations;

namespace ApiEcommerce.Models.Dtos
{
    public class UserLoginDto
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public required string Username { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public required string Password { get; set; }
    }
}
