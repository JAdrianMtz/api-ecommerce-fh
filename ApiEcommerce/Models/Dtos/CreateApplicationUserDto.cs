using System.ComponentModel.DataAnnotations;

namespace ApiEcommerce.Models.Dtos
{
    public class CreateApplicationUserDto : ApplicationUserLoginDto
    {
        [Required]
        public required string UserName { get; set; }
        public string? Name { get; set; }
    }
}
