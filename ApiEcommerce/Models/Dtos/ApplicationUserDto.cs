namespace ApiEcommerce.Models.Dtos
{
    public class ApplicationUserDto
    {
        public required string Email { get; set; }
        public required string Username { get; set; }
        public string? Name { get; set; }
    }
}
