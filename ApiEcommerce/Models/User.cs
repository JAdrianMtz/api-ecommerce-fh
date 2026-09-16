using System.ComponentModel.DataAnnotations;

namespace ApiEcommerce.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public required string? Name { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public required string Username { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public required string Password { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public required string Role { get; set; }
    }
}
