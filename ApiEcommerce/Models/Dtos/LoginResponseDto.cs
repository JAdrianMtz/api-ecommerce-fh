namespace ApiEcommerce.Models.Dtos
{
    public class LoginResponseDto
    {
        public required string Token { get; set; }
        public DateTime Expiration { get; set; }
    }
}
